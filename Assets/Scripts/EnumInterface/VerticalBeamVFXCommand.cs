using UnityEngine;

public class VerticalBeamVFXCommand : IVFXCommand {
    public Vector2 pos;

    public VerticalBeamVFXCommand( Vector2 pos ) {
        this.pos = pos;
    }

    public VFXType GetVFXType() => VFXType.Beam;
}
