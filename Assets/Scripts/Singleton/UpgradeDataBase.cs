using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class UpgradeDataBase : MonoBehaviour {
    const string upgradeLabel = "Upgrade";

    public List<UpgradeSO> upgradeSOs = new List<UpgradeSO>();

    private Task<List<UpgradeSO>> loadingTask;

    async void Start() {
        await GetUpgrades();
    }

    public async Task<List<UpgradeSO>> GetUpgrades() {
        if ( upgradeSOs.Count > 0 )
            return upgradeSOs;

        // if another call already started loading, wait for it
        if ( loadingTask != null )
            return await loadingTask;

        // otherwise start new load
        loadingTask = LoadUpgradesAsync();
        return await loadingTask;

    }

    async Task<List<UpgradeSO>> LoadUpgradesAsync() {
        var handle = Addressables.LoadAssetsAsync<UpgradeSO>(upgradeLabel, null);
        var result = await handle.Task;
        upgradeSOs.AddRange(result);
        Debug.Log($"[UpgradeManager] Loaded {upgradeSOs.Count} UpgradeSO assets.");
        return upgradeSOs;
    }

    

}
