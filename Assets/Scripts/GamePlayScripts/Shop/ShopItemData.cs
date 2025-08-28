using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewShopItem", menuName = "Shop/Item")]
public class ShopItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int price;

    public List<IItemEffect> effects;
}
