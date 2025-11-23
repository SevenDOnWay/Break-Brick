using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class UpgradeDataBase : MonoBehaviour {
    const string upgradeLabel = "Powerup";

    public List<UpgradeSO> upgradeSOs = new List<UpgradeSO>();

    private Task<List<UpgradeSO>> loadingTask;

    public async Task<List<UpgradeSO>> GetCharacters() {
        if ( upgradeSOs.Count > 0 )
            return upgradeSOs;

        // if another call already started loading, wait for it
        if ( loadingTask != null )
            return await loadingTask;

        // otherwise start new load
        loadingTask = LoadCharactersAsync();
        return await loadingTask;

    }

    async Task<List<UpgradeSO>> LoadCharactersAsync() {
        var handle = Addressables.LoadAssetsAsync<UpgradeSO>(upgradeLabel, null);
        var result = await handle.Task;
        upgradeSOs.AddRange(result);
        Debug.Log($"[UpgradeManager] Loaded {upgradeSOs.Count} UpgradeSO assets.");
        return upgradeSOs;
    }


}
