using UnityEngine;

public class LightningProcess : Process {

    const int brickLayer = 1 << 7;

    public override ProcessType GetProssType() => ProcessType.Lightning;

    public override int OnHit( StatManager statManager, BrickScript brick ) {
        float lightningChance = statManager.GetStat(UpgradeType.LightningChance);
        int maxBounces = Mathf.FloorToInt(statManager.GetStat(UpgradeType.LightningBounces));

        float roll = Random.Range(0f, 1f);

        if ( roll >= lightningChance ) return 0;

        ArcLightning(brick.transform.position, maxBounces);

        return 1;
    }

    void ArcLightning( Vector2 origin, int maxBounces ) {
        const float arcRadius = 2.5f;
        Vector2 currentPos = origin;

        for ( int i = 0; i < maxBounces; i++ ) {
            Collider2D[] hits = Physics2D.OverlapCircleAll(currentPos, arcRadius, brickLayer);

            BrickScript closest = null;
            float closestDist = float.MaxValue;

            foreach ( var hit in hits ) {
                float dist = Vector2.Distance(currentPos, hit.transform.position);
                if ( dist < closestDist && dist > 0.1f ) {
                    var brick = hit.GetComponent<BrickScript>();
                    if ( brick != null && !brick.IsDead ) {
                        closest = brick;
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
