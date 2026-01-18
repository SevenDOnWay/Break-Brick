using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class UpgradeUI : MonoBehaviour {
    private UpgradeManager upgradeManager;

    [Header("UI References")]
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private Transform optionsContainer; // Container for upgrade option buttons
    [SerializeField] private UpgradeOptionUI optionPrefab; // The prefab for a single upgrade button

    // Store instantiated buttons to clean them up later
    private List<UpgradeOptionUI> activeOptions = new List<UpgradeOptionUI>();

    private bool isShowingOptions = false;

    [Inject]
    void Constructor(
        UpgradeManager upgradeManager
     ) {
        this.upgradeManager = upgradeManager;
    }

    void Start() {
        SetUpObservers();
    }

    private void SetUpObservers() {
        upgradeManager.OnUpgradeReady += ShowUpgradeOptions;
    }

    public void ShowUpgradeOptions( UpgradeSO[] upgrades ) {
        //MAYBE: use current Level to influence upgrade choices (need to implement upgrade tiers first)
        StartCoroutine(ShowUpgradeQueueRoutine(upgrades));
    }

    private IEnumerator ShowUpgradeQueueRoutine( UpgradeSO[] upgrades ) {
        yield return new WaitWhile(() => isShowingOptions);

        // Lock the queue immediately so other upgrades know to wait
        isShowingOptions = true;

        ClearOldOptions();
        upgradePanel.SetActive(true);

        foreach ( var upgrade in upgrades ) {
            CreateOption(upgrade);
        }
    }

    private void CreateOption( UpgradeSO upgradeSO ) {
        // Instantiate the prefab inside the container
        UpgradeOptionUI newOption = Instantiate(optionPrefab, optionsContainer);

        // Initialize the button with data and a callback
        newOption.Setup(upgradeSO, OnUpgradeSelected);

        activeOptions.Add(newOption);

    }

    private void OnUpgradeSelected( UpgradeSO selectedUpgrade ) {
        upgradeManager.ApplyUpgrade(selectedUpgrade);

        HideUI();
        isShowingOptions = false;
    }

    private void ClearOldOptions() {
        foreach ( var option in activeOptions ) {
            Destroy(option.gameObject);
        }
        activeOptions.Clear();
    }

    private void HideUI() {
        upgradePanel.SetActive(false);
        ClearOldOptions();
    }

}
