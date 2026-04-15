using System;
using UnityEngine;

public abstract class VFXPlayerBase : MonoBehaviour, IVFXPlayer {

    public abstract VFXType GetVFXType();
    public void Execute( IVFXCommand cmd, Action onComplete) {
        gameObject.SetActive(true);
        cmd.ExecuteOn(this, onComplete);

        //Debug.Log($"[VFXPlayerBase] Executing VFX Command of type {cmd.GetVFXType()} on player of type {GetVFXType()}");
    }

    public virtual void PlayExplosion( IVFXCommand cmd, Action onComplete ) { }
    public virtual void PlayHorizontalBeam( IVFXCommand cmd, Action onComplete ) { }

    public virtual void PlayVerticalBeam( IVFXCommand cmd, Action onComplete ) { }

    public virtual void Play( Vector2 position, Action onComplete ) { }

    public virtual void Play( Vector2 start, Vector2 end, Action onComplete ) { }

}
