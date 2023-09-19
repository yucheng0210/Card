using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenu : UIBase
{
    [SerializeField]
    private DialogSystem shopDialog;

    [SerializeField]
    private BackpackSlot backpackSlot;

    [SerializeField]
    private Transform slotGroupTrans;

    [SerializeField]
    private Text itemInfoText;

    [SerializeField]
    private Text itemNameText;

    [SerializeField]
    private Text switchButtonText;

    [SerializeField]
    private Text useButtonText;

    [SerializeField]
    private Text moneyText;

    [SerializeField]
    private Button switchButton;

    [SerializeField]
    private Button useButton;
    private bool switchBool;

    protected override void Start()
    {
        base.Start();
        switchButton.onClick.AddListener(SellSwitch);
       /* EventManager.Instance.AddEventRegister(EventDefinition.eventOnClickedToBag, EventOnClicked);
        EventManager.Instance.AddEventRegister(
            EventDefinition.eventReviseMoneyToBag,
            EventReviseMoney
        );*/
    }

    private void Update()
    {
        /*if (shopDialog.OpenMenu && !OpenBool)
        {
            Show();
            shopDialog.OpenMenu = false;
            UIManager.Instance.RefreshItem(
                backpackSlot,
                slotGroupTrans,
                DataManager.Instance.ShopBag
            );
            UpdateItemInfo("", "");
        }*/
    }

    public void Business(Item item)
    {
       /* if (!switchBool)
        {
            if (DataManager.Instance.MoneyCount >= item.ItemCost)
            {
                BackpackManager.Instance.ReduceMoney(item.ItemBuyPrice);
                BackpackManager.Instance.AddItem(item.ItemIndex, DataManager.Instance.Backpack);
                BackpackManager.Instance.ReduceItem(item.ItemIndex, DataManager.Instance.ShopBag);
                UIManager.Instance.RefreshItem(
                    backpackSlot,
                    slotGroupTrans,
                    DataManager.Instance.ShopBag
                );
            }
        }
        else
        {
            BackpackManager.Instance.AddMoney(item.ItemSellPrice);
            BackpackManager.Instance.AddItem(item.ItemIndex, DataManager.Instance.ShopBag);
            BackpackManager.Instance.ReduceItem(item.ItemIndex, DataManager.Instance.Backpack);
            UIManager.Instance.RefreshItem(
                backpackSlot,
                slotGroupTrans,
                DataManager.Instance.Backpack
            );
        }*/
    }

    public void UpdateItemInfo(string itemDes, string itemName)
    {
        itemInfoText.text = itemDes;
        itemNameText.text = itemName;
    }

    public void SellSwitch()
    {
       /* if (switchBool)
        {
            UIManager.Instance.RefreshItem(
                backpackSlot,
                slotGroupTrans,
                DataManager.Instance.ShopBag
            );
            switchButtonText.text = "賣";
            useButtonText.text = "買";
            switchBool = false;
        }
        else
        {
            UIManager.Instance.RefreshItem(
                backpackSlot,
                slotGroupTrans,
                DataManager.Instance.Backpack
            );
            switchButtonText.text = "買";
            useButtonText.text = "賣";
            switchBool = true;
        }*/
    }

    public void EventOnClicked(params object[] args)
    {
        UpdateItemInfo(((Item)args[0]).ItemInfo.ToString(), ((Item)args[0]).ItemName.ToString());
        useButton.onClick.RemoveAllListeners();
        useButton.onClick.AddListener(() =>
        {
            Business((Item)args[0]);
        });
    }

    public void EventReviseMoney(params object[] args)
    {
        moneyText.text = args[0].ToString();
    }
}
