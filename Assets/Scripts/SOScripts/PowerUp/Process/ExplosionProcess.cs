using System;
using UnityEngine;
using VContainer;

public class ExplosionProcess : Process {
    [Inject]
    public PlayScreen playScreen { get; set; }

    int brickLayer = 1 << 7;

    //public void RegisterVFXEvents( VFXManager vfxManager ) {
    //    OnExplose += vfxManager.PlayExplosionVFX;
    //}

    public override ProcessType GetProssType() => ProcessType.Explosion;

    protected override float GetChance( StatManager statManager ) {
        return statManager.GetStat(UpgradeType.ExplosionChance);
    }

    protected override int Execute( StatManager statManager, BrickScript brick, int baseDamage ) {
        float logicalRadius = statManager.GetStat(UpgradeType.ExplosionRadius);
        float physicalRadius = logicalRadius * (playScreen?.GetSquareSize() ?? 1f);
        Vector2 pos = brick.transform.position;
        Explose(pos, physicalRadius);

        RaiseVFXCommand(new ExplosionVFXCommand(pos, physicalRadius));

        return 1; //MAYBE: add some damage boost for explosion hit
    }

    public void RaiseVFXCommand( IVFXCommand cmd ) {
        VFXEvent.RaiseVFXCommand(cmd);
    }

    private void Explose( Vector2 pos, float explosionRadius ) {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll( pos, explosionRadius, brickLayer );

        foreach ( var hitCollider in hitColliders ) {
            BrickScript brick = hitCollider.GetComponent<BrickScript>();
            if ( brick != null ) {
                brick.NotifyHit(DamageSource.Explosion, 1);
            }
        }

    }



}
