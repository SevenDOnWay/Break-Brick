using DG.Tweening;
using System;
using UnityEngine;
using VContainer;

public class BeamVFXPlayer : VFXPlayerBase {

    IPlayFieldMetrics playFieldMetrics;

    Action onCompleteCallback;
    [SerializeField] LineRenderer lr;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] float beamSpeed;

    private Vector3[] _linePoints = new Vector3[3];

    private Material _mat;
    private Tween _growthTween;

    const int maxDistance = 18;


    [Inject]
    public void Constructor( IPlayFieldMetrics playFieldMetrics ) {
        this.playFieldMetrics = playFieldMetrics;
    }

    void Start() {
        _mat = lr.material;
        float squareSize = playFieldMetrics?.SquareSize ?? 1f;
        lr.startWidth = squareSize * 0.4f;
        lr.endWidth = squareSize * 0.3f;
    }

    public override void Play( IVFXCommand command, Action onComplete ) {
        if ( command is not BeamVFXCommand beam ) {
            Debug.LogWarning($"BeamVFXPlayer cannot render {command?.GetType().Name}.");
            onComplete?.Invoke();
            return;
        }

        _growthTween?.Kill();
        onCompleteCallback = onComplete;

        PlayBeam(beam.Position, beam.Axis, onCompleteCallback);
    }

    void PlayBeam( Vector2 origin, Vector2 direction, Action onComplete ) {

        _linePoints[0] = origin;
        _linePoints[1] = origin;
        _linePoints[2] = origin;
        lr.SetPositions(_linePoints);

        Vector3 targetA = origin + (direction * maxDistance);
        Vector3 targetB = origin + (-direction * maxDistance);

        RaycastHit2D hitA = Physics2D.Raycast(origin, direction, maxDistance, wallLayer);
        if ( hitA.collider != null ) {
            targetA = hitA.point;
        }

        RaycastHit2D hitB = Physics2D.Raycast(origin, -direction, maxDistance, wallLayer);
        if ( hitB.collider != null ) {
            targetB = hitB.point;
        }

        float leftDist = Vector3.Distance(origin, targetA);
        float rightDist = Vector3.Distance(origin, targetB);
        float duration = Mathf.Max(leftDist, rightDist) / beamSpeed;

        _growthTween = DOTween.To(() => 0f, x => {
            _linePoints[0] = Vector3.Lerp(origin, targetA, x);
            _linePoints[2] = Vector3.Lerp(origin, targetB, x);
            lr.SetPositions(_linePoints);
        }, 1f, duration)
        .SetEase(Ease.OutQuad) // Makes the "spam" feel snappier
            .OnComplete(() => onComplete?.Invoke());
    }
}
