using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public int ItemID { get; set; }
    public string ItemName { get; set; }
    public string ItemImagePath { get; set; }
    public string ItemInfo { get; set; }
    public int ItemBuyPrice { get; set; }
    public int ItemSellPrice { get; set; }
    public string ItemEffectName { get; set; }
    public string ItemRarity { get; set; }
    public string ItemType { get; set; }
    public int ItemHeld { get; set; }
    public Item DeepClone()
    {
        Item clone = (Item)this.MemberwiseClone();
        return clone;
    }
}
