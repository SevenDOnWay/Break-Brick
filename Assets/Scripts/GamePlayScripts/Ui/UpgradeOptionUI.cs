using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeOptionUI : MonoBehaviour {

    [Header("UI Elements")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button selectButton;
    [SerializeField] private Sprite defaultIcon;

    private UpgradeSO upgradeSO;
    private Action<UpgradeSO> onSelectedCallback;

    public void Setup( UpgradeSO upgrade, Action<UpgradeSO> onClickCallback ) {
        this.upgradeSO = upgrade;
        this.onSelectedCallback = onClickCallback;

        // Visual setup
        iconImage.sprite = upgrade.GetIcon() != null ? upgrade.GetIcon() : defaultIcon;

        if ( nameText != null ) nameText.text = upgrade.GetUpgradeName();
        if ( descriptionText != null ) descriptionText.text = upgrade.GetDescription();

        // Listener setup
        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(HandleClick);
    }

    private void HandleClick() {
        onSelectedCallback?.Invoke(upgradeSO);
    }
}
