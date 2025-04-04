using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackpackManager : Singleton<BackpackManager>
{
    public void AddPotion(int potionIndex)
    {
        List<Potion> potionBag = DataManager.Instance.PotionBag;
        Dictionary<int, Potion> potionList = DataManager.Instance.PotionList;
        if (potionBag.Count < 4)
        {
            potionBag.Add(potionList[potionIndex]);
        }
    }
    public void UseItem(int itemIndex)
    {
        Item item = DataManager.Instance.ItemList[itemIndex].DeepClone();
        if (!item.CanUse)
        {
            return;
        }
        Texture2D texture2D = Resources.Load<Texture2D>(item.ItemImagePath + "Cursor");
        ReduceItem(itemIndex, DataManager.Instance.Backpack);
        Cursor.SetCursor(texture2D, Vector2.zero, CursorMode.Auto);
        UIManager.Instance.HideUI("BagMenu");
        EventManager.Instance.DispatchEvent(EventDefinition.eventUseItem, item);
    }

    public void AddItem(int itemIndex, Dictionary<int, Item> inventory)
    {
        if (!inventory.ContainsKey(itemIndex))
        {
            inventory.Add(itemIndex, DataManager.Instance.ItemList[itemIndex]);
            inventory[itemIndex].ItemHeld++;
        }
        else
        {
            inventory[itemIndex].ItemHeld++;
        }
        EventManager.Instance.DispatchEvent(EventDefinition.eventAddItemToBag);
    }

    public void ReduceItem(int itemIndex, Dictionary<int, Item> inventory)
    {
        inventory[itemIndex].ItemHeld--;
        if (inventory[itemIndex].ItemHeld <= 0)
        {
            inventory.Remove(itemIndex);
        }
        EventManager.Instance.DispatchEvent(EventDefinition.eventAddItemToBag);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRemoveItemToBag);
    }

    /* public void SetShortcutBar(Item item)
     {
         if (!DataManager.Instance.ShortcutBar.ContainsKey(item.ItemIndex))
             DataManager.Instance.ShortcutBar.Add(item.ItemIndex, item);
     }*/

    /*public void RemoveAllItem()
    {
        UpdateItemInfo("");
        myBag.ItemList.RemoveAll();
        RefreshItem();
    }*/

    public void AddMoney(int count)
    {
        DataManager.Instance.MoneyCount += count;
        /* EventManager.Instance.DispatchEvent(
             EventDefinition.eventReviseMoneyToBag,
             DataManager.Instance.MoneyCount
         );*/
    }

    public void ReduceMoney(int count)
    {
        DataManager.Instance.MoneyCount -= count;
        /* EventManager.Instance.DispatchEvent(
             EventDefinition.eventReviseMoneyToBag,
             DataManager.Instance.MoneyCount
         );*/
    }

    public int GetMoney()
    {
        return DataManager.Instance.MoneyCount;
    }
}
