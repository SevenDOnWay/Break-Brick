using System;
using UnityEngine;

public class BeamVFXCommand : IVFXCommand {

    public Vector2 pos;

    public BeamVFXCommand( Vector2 pos ) {
        this.pos = pos;
    }

    public void ExecuteOn( VFXPlayerBase player, Action onComplete ) {
        var cmd = new BeamVFXCommand(pos);
        player.PlayHorizontalBeam(cmd, onComplete);
    }

    public VFXType GetVFXType() => VFXType.HorizontalBeam;
}
