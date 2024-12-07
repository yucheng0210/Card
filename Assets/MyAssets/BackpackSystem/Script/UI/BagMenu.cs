using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagMenu : UIBase
{
    [SerializeField]
    private Transform slotGroupTrans;

    [SerializeField]
    private BackpackSlot slotPrefab;

    [SerializeField]
    private Text moneyText;

    [SerializeField]
    private Text itemNameText;

    [SerializeField]
    private Text itemInfoText;

    [SerializeField]
    private Button useButton;
    [SerializeField]
    private Button showButton;
    [SerializeField]
    private Button hideButton;

    protected override void Start()
    {
        base.Start();
        showButton.onClick.AddListener(Show);
        hideButton.onClick.AddListener(Hide);
        EventManager.Instance.AddEventRegister(EventDefinition.eventAddItemToBag, EventAddItem);
        EventManager.Instance.AddEventRegister(EventDefinition.eventRemoveItemToBag, EventRemoveItem);
        EventManager.Instance.AddEventRegister(EventDefinition.eventReviseMoneyToBag, EventReviseMoney);
        EventManager.Instance.AddEventRegister(EventDefinition.eventOnClickedToBag, EventOnClicked);
    }

    public override void Show()
    {
        base.Show();
        UIManager.Instance.RefreshItem(slotPrefab, slotGroupTrans, DataManager.Instance.Backpack);
        UpdateItemInfo("", "");
    }

    public void UpdateItemInfo(string itemDes, string itemName)
    {
        itemInfoText.text = itemDes;
        itemNameText.text = itemName;
    }

    public void EventAddItem(params object[] args)
    {
        UIManager.Instance.RefreshItem(slotPrefab, slotGroupTrans, DataManager.Instance.Backpack);
    }

    public void EventRemoveItem(params object[] args)
    {
        UpdateItemInfo("", "");
        UIManager.Instance.RefreshItem(slotPrefab, slotGroupTrans, DataManager.Instance.Backpack);
    }

    public void EventReviseMoney(params object[] args)
    {
        moneyText.text = args[0].ToString();
    }

    public void EventOnClicked(params object[] args)
    {
        UpdateItemInfo(((Item)args[0]).ItemInfo.ToString(), ((Item)args[0]).ItemName.ToString());
        useButton.onClick.RemoveAllListeners();
        useButton.onClick.AddListener(() =>
        {
            BackpackManager.Instance.UseItem(((Item)args[0]).ItemID);
        });
    }
}
