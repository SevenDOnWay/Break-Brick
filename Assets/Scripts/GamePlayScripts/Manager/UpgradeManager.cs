using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;

public class UpgradeManager {
    RunDataManager runDataManager;
    UpgradeDataBase upgradeDataBase;
    StatManager statManager;



    [Header("List")]
    readonly List<UpgradeSO> upgradeSODataBase = new List<UpgradeSO>();
    List<UpgradeSO> currentUpgrades = new List<UpgradeSO>();
    List<Process> currentProcess= new List<Process>();


    [Inject]
    public void Construct(
        RunDataManager runDataManager,
        StatManager statManager,
        UpgradeDataBase upgradeDataBase
        ) {
        this.runDataManager = runDataManager;
        this.statManager = statManager;
        this.upgradeDataBase = upgradeDataBase;
    }

    //TODO: Initialize UpgradeManager

    //TODO: Get Random Upgrades for shop

    //TODO: Apply Selected Upgrade
    public void ApplyUpgrade( UpgradeSO upgrade ) {

        if ( upgrade is UpgradeStatSO statUpgrade ) {

            foreach ( var kvp in statUpgrade.GetKeyValueMap() ) {
                statManager.ModifyStat(kvp.Key, kvp.Value);
                CheckForProcess(kvp.Key);
            }

        }
        else if ( upgrade is UpgradeBehaviorSO behaviorUpgrade ) {
            behaviorUpgrade.ApplyBehavior(this);
        }

    }

    /// <summary>
    /// Check if upgrade type triggers any process to be added
    /// </summary>
    public void CheckForProcess( UpgradeType type ) {
        switch ( type ) {
            case UpgradeType.CritChance:
                currentProcess.Add(new CritProcess());
                break;
            //TODO: add other processes

            //case UpgradeType.FireChance:
            //    currentProcess.Add(new FireProcess());
            //    break;
            //case UpgradeType.LightningChance:
            //    currentProcess.Add(new LightningProcess());
            //    break;
            default:
                break;
        }
    }

    //TODO: Add Process when applying upgrade

    //TODO: Get Current Upgrades

    //TODO: Get Current Processes

    //TODO: Clear Current Upgrades and Processes


    //TODO: Save current upgrades to rundata
    #region Save
    #endregion

    //TODO: Read rundatat and restore upgrades
    #region Restore

    public void RestoreUpgrades() {

    }

    #endregion
}
