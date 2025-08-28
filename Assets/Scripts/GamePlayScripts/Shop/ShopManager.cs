using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShopManager : MonoBehaviour
{
    [SerializeField] private List<ShopItemData> items;

    [SerializeField] private GameObject itemUIPrefab;

    void Start()
    {
        foreach (var item in items)
        {
            GameObject obj = Instantiate(itemUIPrefab, transform);


            ShopItemUI ui = obj.GetComponent<ShopItemUI>();
            ui.SetData(item);

            Button buyButton = obj.GetComponent<Button>();
            buyButton.onClick.AddListener(ui.OnBuy);
        }
    }
}
