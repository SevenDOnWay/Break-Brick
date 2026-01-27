using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ExplosionProcess : Process, IVFXEvent {

    int brickLayer = 1 << 7;

    public event Action<Vector2, float> OnExplose;

    //public void RegisterVFXEvents( VFXManager vfxManager ) {
    //    OnExplose += vfxManager.PlayExplosionVFX;
    //}

    public override ProcessType GetProssType() => ProcessType.Explosion;

    public override int OnHit( StatManager statManager , Vector2 pos ) {
        float explosionChance = statManager.GetStat( UpgradeType.ExplosionChance );
        float explosionRadius = statManager.GetStat( UpgradeType.ExplosionRadius );

        float roll = UnityEngine.Random.Range( 0f, 1f );

        if ( roll >= explosionChance ) return 1;

        Explose(pos, explosionRadius);

        return 1; //MAYBE: add some damage boost for explosion hit
    }

    public void RegisterVFXEvents( VFXManager vfxManager ) {
        throw new NotImplementedException();
    }

    private void Explose( Vector2 pos, float explosionRadius ) {
        Collider[] hitColliders = Physics.OverlapSphere( pos, explosionRadius, brickLayer );

        //TODO: add explosion VFX 
        OnExplose?.Invoke(pos, explosionRadius);


        foreach ( var hitCollider in hitColliders ) {
            BrickScript brick = hitCollider.GetComponent<BrickScript>();
            if ( brick != null ) {
                brick.TakeDamage(1);
            }
        }

    }



}
