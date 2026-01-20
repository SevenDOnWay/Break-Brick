using UnityEngine;

public interface IVFXCommand {
    /// <summary>
    /// this will be called when the command is executed on the player
    /// </summary>
    /// <param name="player"></param>
    void ExecuteOn(IVFXPlayerBase player);
}
