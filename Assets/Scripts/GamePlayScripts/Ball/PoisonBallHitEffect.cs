using UnityEngine;

[CreateAssetMenu(fileName = "PoisonBallHitEffect", menuName = "Break Brick/Ball Effects/Poison")]
public class PoisonBallHitEffect : BallHitEffect {
    [SerializeField, Min(1)] int durationTurns = 3;
    [SerializeField, Min(1)] int damagePerTick = 1;

    public override BallType BallType => BallType.Poison;

    protected override void Execute( BallHitContext context ) {
        context.brick.ApplyOrRefreshEffect(new PoisonEffect(durationTurns, damagePerTick));
        VFXEvent.RaiseVFXCommand(new PoisonVFXCommand(context.brick.transform.position));
    }
}
