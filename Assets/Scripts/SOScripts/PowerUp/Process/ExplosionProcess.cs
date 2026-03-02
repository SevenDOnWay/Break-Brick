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

    public override int OnHit( StatManager statManager , Vector2 pos ) {
        float explosionChance = statManager.GetStat( UpgradeType.ExplosionChance );
        float explosionRadius = statManager.GetStat( UpgradeType.ExplosionRadius );

        float roll = UnityEngine.Random.Range( 0f, 1f );

        if ( roll >= explosionChance ) return 0; //no explosion

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
