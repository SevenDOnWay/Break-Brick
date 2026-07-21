using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;

public class UpgradeManager {
    RunDataManager runDataManager;
    StatManager statManager;
    ProcessFactory processFactory;
    CharacterDataBase characterDataBase;
    UpgradeDataBase upgradeDataBase;


    [Header("List")]
    readonly List<UpgradeSO> upgrades = new List<UpgradeSO>();
    List<UpgradeSO> currentUpgrades = new List<UpgradeSO>();
    List<Process> currentProcess= new List<Process>();
    Queue<int> pendingUpgrades = new Queue<int>();

    public event Action<BallType, int> RequestExtraBalls;
    public event Action<UpgradeSO[]> OnUpgradeReady;
    public event Action<Process> OnProcessAdded;
    public event Action<UpgradeSO> OnUpgradeAdded;
    public event Action OnAllUpgradesProcessed;
    //public event Action<Process> OnProcessRemoved;
    //public event Action<UpgradeSO> OnUpgradeRemoved;


    private bool isUpgradeMenuOpen = false;



    [Inject]
    public void Construct(
        RunDataManager runDataManager,
        StatManager statManager,
        ProcessFactory processFactory,
        UpgradeDataBase upgradeDataBase,
        CharacterDataBase characterDataBase
        ) {
        this.runDataManager = runDataManager;
        this.statManager = statManager;
        this.processFactory = processFactory;
        this.upgradeDataBase = upgradeDataBase;
        this.characterDataBase = characterDataBase;
    }
    //TODO: Load Character upgrade from run data

    public async void StartGame() {
        await CacheAllUpgrades();
        _ = LoadCharacterUpgrade();
        //SetUpObserver(); //nothing for now
    }

    #region DataBase

    public async Task CacheAllUpgrades() {
        upgrades.Clear();

        var loaded = await upgradeDataBase.GetUpgrades();
        upgrades.AddRange(loaded);
    }

    public async Task LoadCharacterUpgrade() {
        string upgradeIds = runDataManager.runData.GetCharacterSOId();

        CharacterSO characterSO = await characterDataBase.GetCharacterByID(upgradeIds);

        IReadOnlyList<UpgradeStatSO.UpgradePair> statPairs = characterSO?.Apply(statManager, this);
        RegisterProcessesAndActions(statPairs);

    }

    #endregion

    //public void SetUpObserver() {

    //}

    //TODO: Initialize UpgradeManager


    public void ApplyUpgrade( UpgradeSO upgrade ) {
        Debug.Log($"Applied upgrade: {upgrade.name}");

        if ( upgrade is UpgradeStatSO statUpgrade ) {
            currentUpgrades.Add(statUpgrade);
            IReadOnlyList<UpgradeStatSO.UpgradePair> statPairs = statUpgrade.ApplyStat(statManager);
            RegisterProcessesAndActions(statPairs);
        }
        else if ( upgrade is UpgradeBehaviorSO behaviorUpgrade ) {
            currentUpgrades.Add(behaviorUpgrade);
            behaviorUpgrade.ApplyBehavior(this);
        }
        else {
            Debug.LogWarning("UpgradeManager: Unknown upgrade type."); //Fallback just in case
            return;
        }

        isUpgradeMenuOpen = false;
        TryShowNextUpgrade();
    }

    public void ApplyProcess( Process process ) {
        if(currentProcess.Exists(p => p.GetType() == process.GetType()) ) return; // Prevent duplicate processes of the same type. This is a simple check, you might want to implement a more robust system depending on your needs.
        currentProcess.Add(process);
        OnProcessAdded?.Invoke(process);
    }

    void RegisterProcessesAndActions( IReadOnlyList<UpgradeStatSO.UpgradePair> statPairs ) {
        if ( statPairs == null ) {
            return;
        }

        foreach ( UpgradeStatSO.UpgradePair pair in statPairs ) {
            if ( pair.Type == UpgradeType.ExtraBalls ) {
                AddBall(pair.BallType, (int)pair.Value);
                continue;
            }

            Process process = processFactory.CreateProcess(pair.Type);
            if ( process != null ) {
                ApplyProcess(process);
            }
        }
    }

    //public void OnProcessAddedInvoke( Process process ) {
    //    OnProcessAdded?.Invoke(process);
    //}

    public List<Process> GetAllProcess() => currentProcess;

    public void AddBall( BallType ballType, int extraBall ) {
        RequestExtraBalls?.Invoke(ballType, extraBall);
    }


    //TODO: Get Current Upgrades

    //TODO: Get Current Processes


    #region Setup Upgrade UI
    //TODO: Call upgradeUI to show upgrade options
    public void SetUpUpgrade( int currentLevel ) {
        pendingUpgrades.Enqueue(currentLevel);

        TryShowNextUpgrade();
    }

    private void TryShowNextUpgrade() {
        if ( isUpgradeMenuOpen ) return;
        if ( pendingUpgrades.Count == 0 ) {
            Debug.Log("All pending upgrades finished. Returning control to GameState.");
            OnAllUpgradesProcessed?.Invoke();
            return;
        }


        isUpgradeMenuOpen = true;
        // MAYBE currnet level influce upgrade choices (need upgrade tier list)
        int levelForUpgrade = pendingUpgrades.Dequeue();
        Debug.Log($"Showing Upgrades for Level {levelForUpgrade}...");

        var options = GetRandomUpgrade();
        OnUpgradeReady?.Invoke(options);
    }

    public UpgradeSO[] GetRandomUpgrade() {
        int countToReturn = Mathf.Min(3, upgrades.Count);

        List<UpgradeSO> pool = new List<UpgradeSO>(upgrades);
        UpgradeSO[] result = new UpgradeSO[countToReturn];

        for ( int i = 0; i < countToReturn; i++ ) {
            int randomIndex = UnityEngine.Random.Range(0, pool.Count);

            result[i] = pool[randomIndex];

            // Remove from pool so it can't be picked again
            int lastIndex = pool.Count - 1;
            pool[randomIndex] = pool[lastIndex];
        }

        return result;
    }
    #endregion

    //TODO: Clear Current Upgrades and Processes
    public void ClearUpgradesAndProcesses() {
        currentUpgrades.Clear();
        currentProcess.Clear();
    }

    //TODO: Save current upgrades to rundata
    #region Save
    #endregion

    //TODO: Read rundatat and restore upgrades
    #region Restore

    public void RestoreUpgrades() {}

    #endregion

    #region Magnet

    bool isMagnetActive = false;

    public void SetMagnetActive( bool active ) {
        isMagnetActive = active;
    }

    public bool IsMagnetActive() => isMagnetActive;

    #endregion
}
