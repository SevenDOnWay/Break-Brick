using System;
using UnityEngine;

public class HorizontalBeamVFXCommand : IVFXCommand {

    public Vector2 pos;

    public HorizontalBeamVFXCommand( Vector2 pos ) {
        this.pos = pos;
    }

    public void ExecuteOn( VFXPlayerBase player, Action onComplete ) {
        var cmd = new HorizontalBeamVFXCommand(pos);
        player.PlayHorizontalBeam(cmd, onComplete);
    }

    public VFXType GetVFXType() => VFXType.Beam;
}
