using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShopManager : MonoBehaviour
{
    [SerializeField] private List<ShopItemData> items;

    [SerializeField] private GameObject itemUIPrefab;

    [SerializeField] private GameObject grid;

    [SerializeField] private Button rollButton;

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

    public void OpenShop()
    {
        gameObject.SetActive(true);
    }


    public void CloseShop()
    {
        gameObject.SetActive(true);
    }
}
