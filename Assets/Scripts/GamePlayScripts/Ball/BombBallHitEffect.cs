using UnityEngine;

[CreateAssetMenu(fileName = "BombBallHitEffect", menuName = "Break Brick/Ball Effects/Bomb")]
public class BombBallHitEffect : BallHitEffect {
    const int BrickLayer = 1 << 7;

    [SerializeField, Min(0.1f)] float radiusCells = 1.5f;
    [SerializeField, Min(0)] int damage = 1;
    [SerializeField] bool includeHitBrick;

    public override BallType BallType => BallType.Bomb;

    protected override void Execute( BallHitContext context ) {
        Vector2 position = context.brick.transform.position;
        float radius = radiusCells * Mathf.Max(context.squareSize, 0.01f);
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, radius, BrickLayer);

        foreach ( var hit in hits ) {
            if ( !hit.TryGetComponent(out BrickScript target) ) continue;
            if ( target == null || target.IsDead ) continue;
            if ( !includeHitBrick && target == context.brick ) continue;

            target.NotifyHit(DamageSource.Explosion, damage);
        }

        VFXEvent.RaiseVFXCommand(new ExplosionVFXCommand(position, radius));
    }
}
