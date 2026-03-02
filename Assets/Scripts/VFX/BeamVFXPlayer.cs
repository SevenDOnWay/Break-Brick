using DG.Tweening;
using System;
using UnityEngine;
using VContainer;

public class BeamVFXPlayer : VFXPlayerBase {

    PlayScreen playScreen;

    Action onCompleteCallback;
    [SerializeField] LineRenderer lr;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] float beamSpeed;

    private Vector3[] _linePoints = new Vector3[3];

    private Material _mat;
    private Tween _growthTween;

    const int maxDistance = 18;


    [Inject]
    public void Constructor( PlayScreen playScreen ) {
        this.playScreen = playScreen;
    }

    void Start() {
        _mat = lr.material;
        lr.startWidth = playScreen.squareSize * 0.4f;
        lr.endWidth = playScreen.squareSize * 0.3f;
    }

    public override void PlayVerticalBeam( IVFXCommand cmd, Action onComplete ) => PrepareAndPlay(cmd, Vector2.up, onComplete);

    public override void PlayHorizontalBeam( IVFXCommand cmd, Action onComplete ) => PrepareAndPlay(cmd, Vector2.left, onComplete);

    private void PrepareAndPlay( IVFXCommand cmd, Vector2 axis, Action onComplete ) {
        Vector2 pos = Vector2.zero;

        if ( cmd is VerticalBeamVFXCommand vCmd ) pos = vCmd.pos;
        else if ( cmd is HorizontalBeamVFXCommand hCmd ) pos = hCmd.pos;
        else return;

        _growthTween?.Kill();
        onCompleteCallback = onComplete;

        Play(pos, axis, onCompleteCallback);
    }

    public void Play( Vector2 origin, Vector2 direction, Action onComplete ) {

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

    public override VFXType GetVFXType() => VFXType.Beam;
}