using UnityEngine;

public class PoisonVFXCommand : IVFXCommand {

    readonly Vector2 position;

    public PoisonVFXCommand( Vector2 position ) {
        this.position = position;
    }

    public VFXType GetVFXType() => VFXType.Poison;

    public Vector2 GetPosition() => position;
}
