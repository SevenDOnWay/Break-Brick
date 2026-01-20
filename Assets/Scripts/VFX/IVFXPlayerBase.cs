using UnityEngine;

public abstract class IVFXPlayerBase : MonoBehaviour, IVFXPlayer{

    public abstract VFXType GetVFXType();

    public virtual void Execute(IVFXCommand cmd) {
        cmd.ExecuteOn(this);
    }

    public virtual void Explosion( Vector3 position, float radius ) { }
    public virtual void HorizontalBeam( Vector3 postion ) { }

}
