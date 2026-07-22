using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class UpgradeUI : MonoBehaviour {
    private IUpgradeSelectionService upgradeSelectionService;

    [Header("UI References")]
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private Transform optionsContainer; // Container for upgrade option buttons
    [SerializeField] private UpgradeOptionUI optionPrefab; // The prefab for a single upgrade button

    // Store instantiated buttons to clean them up later
    private List<UpgradeOptionUI> activeOptions = new List<UpgradeOptionUI>();

    private bool isShowingOptions = false;

    [Inject]
    void Constructor(
        IUpgradeSelectionService upgradeSelectionService
     ) {
        this.upgradeSelectionService = upgradeSelectionService;
    }

    void Start() {
        SetUpObservers();
    }

    private void SetUpObservers() {
        upgradeSelectionService.UpgradeOffersReady += ShowUpgradeOptions;
    }

    public void ShowUpgradeOptions( UpgradeOffer[] upgrades ) {
        //MAYBE: use current Level to influence upgrade choices (need to implement upgrade tiers first)
        StartCoroutine(ShowUpgradeQueueRoutine(upgrades));
    }

    private IEnumerator ShowUpgradeQueueRoutine( UpgradeOffer[] upgrades ) {
        yield return new WaitWhile(() => isShowingOptions);

        // Lock the queue immediately so other upgrades know to wait
        isShowingOptions = true;

        ClearOldOptions();
        upgradePanel.SetActive(true);

        foreach ( var upgrade in upgrades ) {
            CreateOption(upgrade);
        }
    }

    private void CreateOption( UpgradeOffer upgradeOffer ) {
        // Instantiate the prefab inside the container
        UpgradeOptionUI newOption = Instantiate(optionPrefab, optionsContainer);

        // Initialize the button with data and a callback
        newOption.Setup(upgradeOffer, OnUpgradeSelected);

        activeOptions.Add(newOption);

    }

    private void OnUpgradeSelected( string selectedUpgradeId ) {
        upgradeSelectionService.SelectUpgrade(selectedUpgradeId);

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
