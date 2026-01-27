using System;
using UnityEngine;

public interface IVFXCommand{
    
    public VFXType GetVFXType();

    /// <summary>
    /// this will be called command to executed on the player
    /// </summary>
    /// <param name="player"></param>
    void ExecuteOn(VFXPlayerBase player, Action onComplete);
}
