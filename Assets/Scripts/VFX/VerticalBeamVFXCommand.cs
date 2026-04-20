using System;
using UnityEngine;

public class VerticalBeamVFXCommand : IVFXCommand {
    public Vector2 pos;

    public VerticalBeamVFXCommand( Vector2 pos ) {
        this.pos = pos;
    }

    public void ExecuteOn( VFXPlayerBase player, Action onComplete ) {
        var cmd = new VerticalBeamVFXCommand(pos);
        player.PlayVerticalBeam(cmd, onComplete);
    }

    public VFXType GetVFXType() => VFXType.Beam;
}
