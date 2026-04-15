using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ExplosionProcess : Process {

    int brickLayer = 1 << 7;

    //public void RegisterVFXEvents( VFXManager vfxManager ) {
    //    OnExplose += vfxManager.PlayExplosionVFX;
    //}

    public override ProcessType GetProssType() => ProcessType.Explosion;

    public VFXType GetVFXType() {
        throw new NotImplementedException();
    }

    protected override float GetChance( StatManager statManager ) {
        return statManager.GetStat(UpgradeType.ExplosionChance);
    }

    protected override int Execute( StatManager statManager, BrickScript brick ) {
        float explosionRadius = statManager.GetStat(UpgradeType.ExplosionRadius);
        Vector2 pos = brick.transform.position;
        Explose(pos, explosionRadius);

        RaiseVFXCommand(new ExplosionVFXCommand(pos, explosionRadius));

        return 1; //MAYBE: add some damage boost for explosion hit
    }

    public void RaiseVFXCommand( IVFXCommand cmd) {
        VFXEvent.RaiseVFXCommand(cmd);
    }

    private void Explose( Vector2 pos, float explosionRadius ) {
        Collider[] hitColliders = Physics.OverlapSphere( pos, explosionRadius, brickLayer );

        //TODO: add explosion VFX 
        //OBSLETE: change to raise vfx instead


        foreach ( var hitCollider in hitColliders ) {
            BrickScript brick = hitCollider.GetComponent<BrickScript>();
            if ( brick != null ) {
                brick.NotifyHit(DamageSource.Explosion, 1);
            }
        }

    }

}
