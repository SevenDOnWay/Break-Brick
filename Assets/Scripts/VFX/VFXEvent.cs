using System;
using UnityEngine;

public static class VFXEvent {
    public static event Action<IVFXCommand> OnVFXCommand;

    public static void RaiseVFXCommand( IVFXCommand cmd ) {
        Debug.Log($"[VFXEvent] Raising VFX Command of type {cmd.GetVFXType()}");
        OnVFXCommand?.Invoke(cmd);
    }
}
