using System;
using UnityEngine;

public class LightningVFXCommand : IVFXCommand {

    readonly Vector2 startPos;
    readonly Vector2 endPos;

    public LightningVFXCommand( Vector2 startPos, Vector2 endPos ) {
        this.startPos = startPos;
        this.endPos = endPos;
    }

    public VFXType GetVFXType() => VFXType.Lightning;

    public void ExecuteOn( VFXPlayerBase player, Action onComplete ) {
        player.Play(startPos, endPos, onComplete);
    }

    public Vector2 GetStartPos() => startPos;
    public Vector2 GetEndPos() => endPos;
}
