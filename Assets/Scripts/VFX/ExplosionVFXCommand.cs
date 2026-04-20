using System;
using UnityEngine;

public class ExplosionVFXCommand : IVFXCommand {

    public Vector3 position;
    public float radius;

    public ExplosionVFXCommand( Vector3 position, float radius ) {
        this.position = position;
        this.radius = radius;
    }

    public void ExecuteOn( VFXPlayerBase player, Action onComplete ) {
        var cmd = new ExplosionVFXCommand(position, radius);
        player.PlayExplosion(cmd, onComplete);
    }

    public VFXType GetVFXType() => VFXType.Explosion;
    
}
