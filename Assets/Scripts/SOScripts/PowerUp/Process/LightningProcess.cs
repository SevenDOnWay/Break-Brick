using UnityEngine;

public class LightningProcess : Process {

    const int BrickLayer = 1 << 7;

    public override ProcessType GetProssType() => ProcessType.Lightning;

    // Delegates the chance roll to the base class Template Method.
    protected override float GetChance( StatManager statManager ) {
        return statManager.GetStat(UpgradeType.LightningChance);
    }

    protected override int Execute( StatManager statManager, BrickScript brick, int baseDamage ) {
        int maxBounces = Mathf.FloorToInt(statManager.GetStat(UpgradeType.LightningBounces));
        float arcRadius = statManager.GetStat(UpgradeType.LightningArcRadius);

        ArcLightning(brick.transform.position, maxBounces, arcRadius);

        // Lightning deals damage through NotifyHit on chained bricks, not as a
        // bonus on the directly hit brick.
        return 0;
    }

    void ArcLightning( Vector2 origin, int maxBounces, float arcRadius ) {
        Vector2 currentPos = origin;

        for ( int i = 0; i < maxBounces; i++ ) {
            Collider2D[] hits = Physics2D.OverlapCircleAll(currentPos, arcRadius, BrickLayer);

            BrickScript closest = null;
            float closestDist = float.MaxValue;

            foreach ( var hit in hits ) {
                float dist = Vector2.Distance(currentPos, hit.transform.position);
                if ( dist < closestDist && dist > 0.1f ) {
                    var candidate = hit.GetComponent<BrickScript>();
                    if ( candidate != null && !candidate.IsDead ) {
                        closest = candidate;
                        closestDist = dist;
                    }
                }
            }

            if ( closest == null ) break;

            closest.NotifyHit(DamageSource.Lightning, 1);

            Vector2 targetPos = closest.transform.position;
            VFXEvent.RaiseVFXCommand(new LightningVFXCommand(currentPos, targetPos));

            currentPos = targetPos;
        }
    }
}
