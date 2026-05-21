using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LightningBallHitEffect", menuName = "Break Brick/Ball Effects/Lightning")]
public class LightningBallHitEffect : BallHitEffect {
    const int BrickLayer = 1 << 7;

    [SerializeField, Min(1)] int maxBounces = 3;
    [SerializeField, Min(0.1f)] float arcRadiusCells = 2f;
    [SerializeField, Min(0)] int damage = 1;

    public override BallType BallType => BallType.Lightning;

    protected override void Execute( BallHitContext context ) {
        Vector2 currentPos = context.brick.transform.position;
        float arcRadius = arcRadiusCells * Mathf.Max(context.squareSize, 0.01f);
        HashSet<BrickScript> visited = new() { context.brick };

        for ( int i = 0; i < maxBounces; i++ ) {
            Collider2D[] hits = Physics2D.OverlapCircleAll(currentPos, arcRadius, BrickLayer);
            BrickScript closest = null;
            float closestDist = float.MaxValue;

            foreach ( var hit in hits ) {
                if ( !hit.TryGetComponent(out BrickScript candidate) ) continue;
                if ( candidate == null || candidate.IsDead || visited.Contains(candidate) ) continue;

                float dist = Vector2.Distance(currentPos, candidate.transform.position);
                if ( dist < closestDist ) {
                    closest = candidate;
                    closestDist = dist;
                }
            }

            if ( closest == null ) break;

            closest.NotifyHit(DamageSource.Lightning, damage);
            VFXEvent.RaiseVFXCommand(new LightningVFXCommand(currentPos, closest.transform.position));
            visited.Add(closest);
            currentPos = closest.transform.position;
        }
    }
}
