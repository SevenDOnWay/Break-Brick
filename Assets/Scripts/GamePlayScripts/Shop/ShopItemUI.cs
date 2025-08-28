using NUnit.Framework.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Image iconImage;

    [SerializeField] private ShopItemData shopItemData; // data

    public void SetData(ShopItemData item)
    {
        shopItemData = item;
        itemNameText.text = item.name;
        priceText.text = item.price.ToString();
        iconImage.sprite = item.icon;
    }

    public void OnBuy()
    {
        Debug.Log($"Purchased {shopItemData.name} for {shopItemData.price}");
        //TODO: Handle purchase logic, e.g., deduct currency, add item to inventory



        // Activate item effects
        foreach (var effect in shopItemData.effects)
        {
            effect.Activate();
        }
    }
}
