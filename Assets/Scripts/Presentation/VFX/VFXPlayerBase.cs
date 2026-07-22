using System;
using UnityEngine;

public abstract class VFXPlayerBase : MonoBehaviour, IVFXPlayer {

    public abstract VFXType GetVFXType();
    public void Execute( IVFXCommand cmd, Action onComplete) {
        gameObject.SetActive(true);

        switch ( cmd ) {
            case ExplosionVFXCommand:
                PlayExplosion(cmd, onComplete);
                break;
            case HorizontalBeamVFXCommand:
                PlayHorizontalBeam(cmd, onComplete);
                break;
            case VerticalBeamVFXCommand:
                PlayVerticalBeam(cmd, onComplete);
                break;
            case FreezeVFXCommand freeze:
                Play(freeze.GetPosition(), onComplete);
                break;
            case PoisonVFXCommand poison:
                Play(poison.GetPosition(), onComplete);
                break;
            case LightningVFXCommand lightning:
                Play(lightning.GetStartPos(), lightning.GetEndPos(), onComplete);
                break;
            default:
                Debug.LogWarning($"Unsupported VFX command: {cmd?.GetType().Name}");
                onComplete?.Invoke();
                break;
        }

        //Debug.Log($"[VFXPlayerBase] Executing VFX Command of type {cmd.GetVFXType()} on player of type {GetVFXType()}");
    }

    public virtual void PlayExplosion( IVFXCommand cmd, Action onComplete ) { }
    public virtual void PlayHorizontalBeam( IVFXCommand cmd, Action onComplete ) { }

    public virtual void PlayVerticalBeam( IVFXCommand cmd, Action onComplete ) { }

    public virtual void Play( Vector2 position, Action onComplete ) { }

    public virtual void Play( Vector2 start, Vector2 end, Action onComplete ) { }

}
