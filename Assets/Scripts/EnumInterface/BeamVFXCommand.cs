using UnityEngine;

public class BeamVFXCommand : IVFXCommand {
    public Vector2 Position { get; }
    public Vector2 Axis { get; }

    public BeamVFXCommand( Vector2 position, Vector2 axis ) {
        Position = position;
        Axis = axis;
    }

    public VFXType GetVFXType() => VFXType.Beam;
}

public sealed class HorizontalBeamVFXCommand : BeamVFXCommand {
    public HorizontalBeamVFXCommand( Vector2 position ) : base(position, Vector2.left) { }
}
