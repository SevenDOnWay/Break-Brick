using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;

public class UpgradeManager : IUpgradeSelectionService {
    RunDataManager runDataManager;
    StatManager statManager;
    BallManager ballManager;
    CharacterDataBase characterDataBase;
    UpgradeDataBase upgradeDataBase;


    [Header("List")]
    readonly List<UpgradeSO> upgrades = new List<UpgradeSO>();
    List<UpgradeSO> currentUpgrades = new List<UpgradeSO>();
    Queue<int> pendingUpgrades = new Queue<int>();

    public event Action<UpgradeOffer[]> UpgradeOffersReady;
    public event Action<UpgradeSO> OnUpgradeAdded;
    public event Action OnAllUpgradesProcessed;
    //public event Action<Process> OnProcessRemoved;
    //public event Action<UpgradeSO> OnUpgradeRemoved;


    private bool isUpgradeMenuOpen = false;



    [Inject]
    public void Construct(
        RunDataManager runDataManager,
        StatManager statManager,
        BallManager ballManager,
        UpgradeDataBase upgradeDataBase,
        CharacterDataBase characterDataBase
        ) {
        this.runDataManager = runDataManager;
        this.statManager = statManager;
        this.ballManager = ballManager;
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

        if ( characterSO == null ) {
            return;
        }

        ApplyDefinition(characterSO.GetStatUpgrade());
        ApplyDefinition(characterSO.GetBehaviorUpgrade());

    }

    #endregion

    //public void SetUpObserver() {

    //}

    //TODO: Initialize UpgradeManager


    public void ApplyUpgrade( UpgradeSO upgrade ) {
        Debug.Log($"Applied upgrade: {upgrade.name}");

        ApplyDefinition(upgrade);
        _ = runDataManager.SaveHistoryJson();

        isUpgradeMenuOpen = false;
        TryShowNextUpgrade();
    }

    void ApplyDefinition( UpgradeSO upgrade ) {
        if ( upgrade == null ) {
            return;
        }

        currentUpgrades.Add(upgrade);
        ApplyEffect(BuildEffect(upgrade));
        OnUpgradeAdded?.Invoke(upgrade);
    }

    UpgradeEffect BuildEffect( UpgradeSO upgrade ) {
        var statChanges = new List<StatChange>();
        var processTypes = new List<ProcessType>();
        var ballGrants = new List<BallGrant>();

        if ( upgrade is UpgradeStatSO statUpgrade ) {
            IReadOnlyList<UpgradeStatSO.UpgradePair> pairs = statUpgrade.GetKeyValueMap();
            if ( pairs != null ) {
                foreach ( UpgradeStatSO.UpgradePair pair in pairs ) {
                    if ( pair.Type == UpgradeType.ExtraBalls ) {
                        ballGrants.Add(new BallGrant(pair.BallType, (int)pair.Value));
                        continue;
                    }

                    statChanges.Add(new StatChange(pair.Type, pair.Value));
                    if ( TryGetProcessType(pair.Type, out ProcessType processType ) && !processTypes.Contains(processType) ) {
                        processTypes.Add(processType);
                    }
                }
            }
        } else if ( upgrade is UpgradeBehaviorSO behaviorUpgrade && behaviorUpgrade.GetBehaviorTypes()?.Count > 0 ) {
            Debug.LogWarning($"Behavior upgrade '{upgrade.name}' has no supported runtime effect. Magnet has been deprecated.");
        }

        return new UpgradeEffect(statChanges, processTypes, ballGrants);
    }

    void ApplyEffect( UpgradeEffect effect ) {
        foreach ( StatChange statChange in effect.StatChanges ) {
            statManager.ModifyStat(statChange.Type, statChange.Value);
        }

        ballManager.ApplyUpgradeEffect(effect);
    }

    static bool TryGetProcessType( UpgradeType upgradeType, out ProcessType processType ) {
        processType = upgradeType switch {
            UpgradeType.CritChance => ProcessType.Crit,
            UpgradeType.ExplosionChance => ProcessType.Explosion,
            UpgradeType.LightningChance => ProcessType.Lightning,
            UpgradeType.PoisonChance => ProcessType.Poison,
            UpgradeType.FreezeChance => ProcessType.Freeze,
            UpgradeType.SniperInterval => ProcessType.Sniper,
            UpgradeType.ShockwaveChance => ProcessType.Shockwave,
            UpgradeType.RallyBonus => ProcessType.Rally,
            _ => default
        };

        return upgradeType is UpgradeType.CritChance
            or UpgradeType.ExplosionChance
            or UpgradeType.LightningChance
            or UpgradeType.PoisonChance
            or UpgradeType.FreezeChance
            or UpgradeType.SniperInterval
            or UpgradeType.ShockwaveChance
            or UpgradeType.RallyBonus;
    }

    public IReadOnlyList<UpgradeSO> GetCurrentUpgrades() => currentUpgrades;


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

        UpgradeOffersReady?.Invoke(GetRandomUpgradeOffers());
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

    UpgradeOffer[] GetRandomUpgradeOffers() {
        UpgradeSO[] upgrades = GetRandomUpgrade();
        UpgradeOffer[] offers = new UpgradeOffer[upgrades.Length];

        for ( int i = 0; i < upgrades.Length; i++ ) {
            UpgradeSO upgrade = upgrades[i];
            offers[i] = new UpgradeOffer(
                upgrade.GetUpgradeId(),
                upgrade.GetIcon(),
                upgrade.GetUpgradeName(),
                upgrade.GetDescription()
            );
        }

        return offers;
    }

    public void SelectUpgrade( string upgradeId ) {
        UpgradeSO selectedUpgrade = upgrades.Find(upgrade => upgrade != null && upgrade.GetUpgradeId() == upgradeId);
        if ( selectedUpgrade == null ) {
            Debug.LogWarning($"UpgradeManager: no upgrade with id {upgradeId} is available.");
            return;
        }

        ApplyUpgrade(selectedUpgrade);
    }
    #endregion

    //TODO: Clear Current Upgrades and Processes
    public void ClearUpgradesAndProcesses() {
        currentUpgrades.Clear();
        ballManager.ClearUpgradeEffects();
    }

    //TODO: Save current upgrades to rundata
    #region Save
    #endregion

    //TODO: Read rundatat and restore upgrades
    #region Restore

    public void RestoreUpgrades() {}

    #endregion

}
