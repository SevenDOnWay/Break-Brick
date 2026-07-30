using UnityEngine;

public class ExplosionVFXCommand : IVFXCommand {

    public Vector3 position;
    public float radius;

    public ExplosionVFXCommand( Vector3 position, float radius ) {
        this.position = position;
        this.radius = radius;
    }

    public VFXType GetVFXType() => VFXType.Explosion;
    
}
