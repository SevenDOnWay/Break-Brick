using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class UpgradeManager : MonoBehaviour {

    public  List<UpgradeSO> upgradeSOs = new List<UpgradeSO>();

    void Start() {
        Addressables.LoadAssetsAsync<UpgradeSO>("AllMyItems", item => {
            // This is called for each asset loaded
            upgradeSOs.Add(item);
        }).Completed += OnLoadDone;
    }

    void OnLoadDone( UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<IList<UpgradeSO>> obj ) {
        Debug.Log("All assets loaded. Total count: " + upgradeSOs.Count);
    }


}
