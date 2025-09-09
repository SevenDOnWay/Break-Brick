using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShopManager : MonoBehaviour
{
    [SerializeField] private List<ShopItemData> items;

    [SerializeField] private GameObject itemUIPrefab;

    [SerializeField] private GameObject grid;

    private void Awake()
    {
        
    }


    void Start()
    {
        foreach (var item in items)
        {
            GameObject obj = Instantiate(itemUIPrefab, grid.transform);
            ShopItemUI ui = obj.GetComponent<ShopItemUI>();
            ui.SetData(item);

        }
    }

}
