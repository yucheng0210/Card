using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;
public class UIShop : UIBase
{
    [SerializeField]
    private GameObject battleBackground;
    [SerializeField]
    private GameObject skyIslandBackground;
    [SerializeField]
    private Transform cardGroupTrans;
    [SerializeField]
    private Transform potionGroupTrans;
    [SerializeField]
    private Button exitButton;
    [SerializeField]
    private CardItem cardPrefab;
    [SerializeField]
    private Button potionPrefab;
    private bool isBattleMode = false;
    public override void Show()
    {
        base.Show();
        isBattleMode = !UIManager.Instance.UIDict["UISkyIsland"].UI.activeSelf;
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(() =>
        {
            if (isBattleMode)
            {
                BattleManager.Instance.NextLevel(GetType().Name);

            }
            else
            {
                UIManager.Instance.HideUI(GetType().Name);
            }
        });
        RefreshMerchandise();
    }
    private void RefreshMerchandise()
    {
        ClearChildren(cardGroupTrans);
        ClearChildren(potionGroupTrans);
        // 切換背景
        battleBackground.SetActive(isBattleMode);
        skyIslandBackground.SetActive(!isBattleMode);

        // 清空現有物品

        // 產生卡牌
        GenerateCards(6);

        // 產生藥水與道具
        int potionCount = isBattleMode ? 4 : 2;
        GenerateItems(potionCount, true);

        // 只有 SkyIsland 模式才會產生額外的道具
        if (!isBattleMode)
        {
            GenerateItems(potionCount, false);
        }
    }

    private void ClearChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            parent.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void GenerateCards(int count)
    {
        Dictionary<int, CardData> cardList = DataManager.Instance.CardList;
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, cardList.Count);
            KeyValuePair<int, CardData> randomCard = cardList.ElementAt(randomIndex);
            CardData cardData = cardList[randomCard.Key];

            if (cardData.CardType == "詛咒")
            {
                i--;
                continue;
            }

            CardItem cardItem = Instantiate(cardPrefab, cardGroupTrans);
            cardItem.GetComponent<CanvasGroup>().alpha = 1;
            cardItem.MyCardData = cardData;
            cardItem.MyCardData.CardID = randomCard.Key;
            cardItem.CantMove = true;

            // 設定卡牌價格
            Text cardPriceText = cardItem.transform.GetChild(cardItem.transform.childCount - 1).GetComponent<Text>();
            cardPriceText.text = cardData.CardBuyPrice.ToString();

            // 綁定點擊事件
            Button cardButton = cardItem.CardImage.GetComponent<Button>();
            cardButton.onClick.AddListener(() => AddCard(cardItem.MyCardData.CardID, cardItem.CardImage));
        }
    }

    private void GenerateItems(int count, bool isPotion)
    {
        Dictionary<int, Item> itemList = isPotion ? DataManager.Instance.PotionList.ToDictionary(kv => kv.Key, kv => (Item)kv.Value) : DataManager.Instance.ItemList;
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, itemList.Count);
            KeyValuePair<int, Item> randomItem = itemList.ElementAt(randomIndex);
            Item itemData = itemList[randomItem.Key];
            PotionItem potionItem = Instantiate(potionPrefab, potionGroupTrans).GetComponent<PotionItem>();
            Text potionPriceText = potionItem.PriceText;
            potionItem.PotionImage.sprite = Resources.Load<Sprite>(itemData.ItemImagePath);
            potionItem.InfoTitleText.text = itemData.ItemName;
            potionItem.InfoDescriptionText.text = itemData.ItemInfo;
            potionPriceText.text = itemData.ItemBuyPrice.ToString();
            potionItem.PotionButton.onClick.AddListener(() => AddItem(itemData.ItemID, potionItem.PotionImage, potionItem.StatusClueTrans, itemList, isPotion));
            UnityAction unityAction_1 = () =>
            {
                potionItem.InfoGameObject.SetActive(true);
                potionItem.PotionCanvas.overrideSorting = true;
            };
            UnityAction unityAction_2 = () =>
            {
                potionItem.InfoGameObject.SetActive(false);
                potionItem.PotionCanvas.overrideSorting = false;
            };
            BattleManager.Instance.SetEventTrigger(potionItem.PotionEventTrigger, unityAction_1, unityAction_2);
        }
    }

    private void AddCard(int cardID, Image card)
    {
        CardData cardData = DataManager.Instance.CardList[cardID];
        if (DataManager.Instance.MoneyCount < cardData.CardBuyPrice)
        {
            return;
        }
        DataManager.Instance.MoneyCount -= cardData.CardBuyPrice;
        DataManager.Instance.CardBag.Add(cardData);
        card.raycastTarget = false;
        card.transform.parent.GetComponent<CanvasGroup>().alpha = 0;
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }
    private void AddItem(int itemID, Image item, Transform statusClueTrans, Dictionary<int, Item> itemList, bool isPotion)
    {
        Item itemData = itemList[itemID];
        if (DataManager.Instance.MoneyCount < itemData.ItemBuyPrice)
        {
            BattleManager.Instance.ShowCharacterStatusClue(statusClueTrans, "金錢不足", 0);
            return;
        }
        DataManager.Instance.MoneyCount -= itemData.ItemBuyPrice;
        if (isPotion)
        {
            DataManager.Instance.PotionBag.Add((Potion)itemData);
        }
        else
        {
            BackpackManager.Instance.AddItem(itemID, DataManager.Instance.Backpack);
        }
        item.raycastTarget = false;
        item.GetComponent<CanvasGroup>().alpha = 0;
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }
}
