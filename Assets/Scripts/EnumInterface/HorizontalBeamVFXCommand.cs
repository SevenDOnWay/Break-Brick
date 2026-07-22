using UnityEngine;

public class HorizontalBeamVFXCommand : IVFXCommand {

    public Vector2 pos;

    public HorizontalBeamVFXCommand( Vector2 pos ) {
        this.pos = pos;
    }

    public VFXType GetVFXType() => VFXType.Beam;
}
