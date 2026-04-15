using System;
using UnityEngine;

public class PoisonVFXCommand : IVFXCommand {

    readonly Vector2 position;

    public PoisonVFXCommand( Vector2 position ) {
        this.position = position;
    }

    public VFXType GetVFXType() => VFXType.Poison;

    public void ExecuteOn( VFXPlayerBase player, Action onComplete ) {
        player.Play(position, onComplete);
    }

    public Vector2 GetPosition() => position;
}
