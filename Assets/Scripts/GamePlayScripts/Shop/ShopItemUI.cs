
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private Image iconImage;

    [SerializeField] private ShopItemData shopItemData; // data

    private void Awake()
    {
        transform.GetComponent<Button>().onClick.AddListener(OnBuy);
    }

    public void SetData(ShopItemData item)
    {
        shopItemData = item;
        itemNameText.text = item.name;
        iconImage.sprite = item.icon;
    }

    public void OnBuy()
    {
        Debug.Log($"Purchased {shopItemData.name}");
        //TODO: Handle purchase logic, e.g., deduct currency, add item to inventory

    }
}
