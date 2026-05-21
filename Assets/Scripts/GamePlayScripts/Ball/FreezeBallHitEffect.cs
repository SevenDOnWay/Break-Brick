using UnityEngine;

[CreateAssetMenu(fileName = "FreezeBallHitEffect", menuName = "Break Brick/Ball Effects/Freeze")]
public class FreezeBallHitEffect : BallHitEffect {
    [SerializeField, Min(1)] int durationTurns = 1;

    public override BallType BallType => BallType.Freeze;

    protected override void Execute( BallHitContext context ) {
        context.brick.ApplyOrRefreshEffect(new FreezeEffect(durationTurns));
        VFXEvent.RaiseVFXCommand(new FreezeVFXCommand(context.brick.transform.position));
    }
}
