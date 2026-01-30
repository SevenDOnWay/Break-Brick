using DG.Tweening;
using System;
using UnityEngine;
using VContainer;
using static Unity.Burst.Intrinsics.X86.Avx;
using static UnityEngine.UI.Image;

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
    public void Constructor( PlayScreen playScreen) {
        this.playScreen = playScreen;
    }

    void Start() {
        _mat = lr.material;
        lr.startWidth = playScreen.squareSize * 0.4f;
        lr.endWidth = playScreen.squareSize * 0.3f;
    }

    public override void PlayHorizontalBeam( IVFXCommand cmd, Action onComplete ) {
        if ( cmd is not BeamVFXCommand beamCmd ) {
            Debug.LogError("Wrong command type passed to ExplosionVFXPlayer");
            onComplete?.Invoke();
            return;
        }

        onCompleteCallback = onComplete;

        _growthTween?.Kill();
        Vector2 startPos = new Vector2(beamCmd.pos.x, beamCmd.pos.y);

        Play(startPos, onCompleteCallback);
    }

    public void Play( Vector2 origin, Action onComplete ) {

        _linePoints[0] = origin;
        _linePoints[1] = origin;
        _linePoints[2] = origin;
        lr.SetPositions(_linePoints);

        Vector3 leftTarget = origin + (Vector2.left * maxDistance);
        Vector3 rightTarget = origin + (Vector2.right * maxDistance);

        RaycastHit2D hitLeft = Physics2D.Raycast(origin, Vector2.left, maxDistance, wallLayer);
        if ( hitLeft.collider != null ) {
            leftTarget = hitLeft.point;
        }

        RaycastHit2D hitRight = Physics2D.Raycast(origin, Vector2.right, maxDistance, wallLayer);
        if ( hitRight.collider != null ) {
            rightTarget = hitRight.point;
        }

        float leftDist = Vector3.Distance(origin, leftTarget);
        float rightDist = Vector3.Distance(origin, rightTarget);
        float duration = Mathf.Max(leftDist, rightDist) / beamSpeed;

        _growthTween = DOTween.To(() => 0f, x => {
            _linePoints[0] = Vector3.Lerp(origin, leftTarget, x);
            _linePoints[2] = Vector3.Lerp(origin, rightTarget, x);
            lr.SetPositions(_linePoints);
        }, 1f, duration)
        .SetEase(Ease.OutQuad) // Makes the "spam" feel snappier
        .OnComplete(() => onComplete?.Invoke());
    }

    public override VFXType GetVFXType() => VFXType.HorizontalBeam;
}
