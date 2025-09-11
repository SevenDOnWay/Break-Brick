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
        rollButton.onClick.AddListener(RollItems);
    }


    void Start()
    {
        RollItems();
    }

    public void OpenShop()
    {
        gameObject.SetActive(true);
    }


    public void CloseShop()
    {
        gameObject.SetActive(true);
    }

    public void RollItems()
    {
        Debug.Log("Rolling new items...");

        // Clear existing items
        foreach (Transform child in grid.transform)
        {
            Destroy(child.gameObject);
        }
 
        foreach (var item in items.GetRandomElements(6))
        {
            GameObject obj = Instantiate(itemUIPrefab, grid.transform);
            ShopItemUI ui = obj.GetComponent<ShopItemUI>();
            ui.SetData(item);
        }   
    }

}
