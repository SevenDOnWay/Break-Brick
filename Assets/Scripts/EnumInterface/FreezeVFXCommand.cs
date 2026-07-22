using UnityEngine;

public class FreezeVFXCommand : IVFXCommand
{

    readonly Vector2 position;

    public FreezeVFXCommand(Vector2 position)
    {
        this.position = position;
    }

    public VFXType GetVFXType() => VFXType.Freeze;

    public Vector2 GetPosition() => position;
}
