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

    private UpgradeOffer upgradeOffer;
    private Action<string> onSelectedCallback;

    public void Setup( UpgradeOffer upgrade, Action<string> onClickCallback ) {
        this.upgradeOffer = upgrade;
        this.onSelectedCallback = onClickCallback;

        // Visual setup
        iconImage.sprite = upgrade.Icon != null ? upgrade.Icon : defaultIcon;

        if ( nameText != null ) nameText.text = upgrade.Name;
        if ( descriptionText != null ) descriptionText.text = upgrade.Description;

        // Listener setup
        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(HandleClick);
    }

    private void HandleClick() {
        onSelectedCallback?.Invoke(upgradeOffer.Id);
    }
}
