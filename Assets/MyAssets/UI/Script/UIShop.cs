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
    protected override void Start()
    {
        base.Start();
        exitButton.onClick.AddListener(() => BattleManager.Instance.NextLevel("UIShop"));
    }
    public override void Show()
    {
        base.Show();
        if (UIManager.Instance.UIDict["UISkyIsland"].UI.activeSelf)
        {
            RefreshMerchandise(false);
        }
        else
        {
            RefreshMerchandise(true);
        }
    }
    private void RefreshMerchandise(bool isBattleMode)
    {
        // 切換背景
        battleBackground.SetActive(isBattleMode);
        skyIslandBackground.SetActive(!isBattleMode);

        // 清空現有物品
        ClearChildren(cardGroupTrans);
        ClearChildren(potionGroupTrans);

        // 產生卡牌
        GenerateCards(6);

        // 產生藥水與道具
        int potionCount = isBattleMode ? 4 : 2;
        GeneratePotions(potionCount);

        // 只有 SkyIsland 模式才會產生額外的道具
        if (!isBattleMode)
        {
            GenerateItems(2);
        }
    }

    private void ClearChildren(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Destroy(parent.GetChild(i).gameObject);
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

    private void GeneratePotions(int count)
    {
        Dictionary<int, Potion> potionList = DataManager.Instance.PotionList;
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, potionList.Count);
            KeyValuePair<int, Potion> randomPotion = potionList.ElementAt(randomIndex);
            Potion potionData = potionList[randomPotion.Key];
            PotionItem potionItem = Instantiate(potionPrefab, potionGroupTrans).GetComponent<PotionItem>();
            Text potionPriceText = potionItem.PriceText;
            potionItem.PotionImage.sprite = Resources.Load<Sprite>(potionData.ItemImagePath);
            potionItem.InfoTitleText.text = potionData.ItemName;
            potionItem.InfoDescriptionText.text = potionData.ItemInfo;
            potionPriceText.text = potionData.ItemBuyPrice.ToString();
            potionItem.PotionButton.onClick.AddListener(() => AddPotion(potionData.ItemID, potionItem.PotionImage, potionItem.StatusClueTrans));
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

    private void GenerateItems(int count)
    {
        Dictionary<int, Item> itemList = DataManager.Instance.ItemList;
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, itemList.Count);
            KeyValuePair<int, Item> randomItem = itemList.ElementAt(randomIndex);
            Item itemData = itemList[randomItem.Key];

            Button itemButton = Instantiate(potionPrefab, potionGroupTrans); // 使用相同的 prefab
            itemButton.GetComponent<Image>().sprite = Resources.Load<Sprite>(itemData.ItemImagePath);

            // 設定物品價格
            Text itemPriceText = itemButton.transform.GetChild(itemButton.transform.childCount - 1).GetComponent<Text>();
            itemPriceText.text = itemData.ItemBuyPrice.ToString();

            // 綁定點擊事件
            itemButton.onClick.AddListener(() => AddItem(itemData.ItemID, itemButton.GetComponent<Image>()));
        }
    }
    private void AddItem(int potionID, Image potion)
    {
        Item item = DataManager.Instance.ItemList[potionID];
        if (DataManager.Instance.MoneyCount < item.ItemBuyPrice)
        {
            return;
        }
        DataManager.Instance.MoneyCount -= item.ItemBuyPrice;
        BackpackManager.Instance.AddItem(potionID, DataManager.Instance.Backpack);
        potion.raycastTarget = false;
        potion.GetComponent<CanvasGroup>().alpha = 0;
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
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
    private void AddPotion(int potionID, Image potion, Transform statusClueTrans)
    {
        Potion item = DataManager.Instance.PotionList[potionID];
        if (DataManager.Instance.MoneyCount < item.ItemBuyPrice)
        {
            BattleManager.Instance.ShowCharacterStatusClue(statusClueTrans, "金錢不足", 0);
            return;
        }
        DataManager.Instance.MoneyCount -= item.ItemBuyPrice;
        DataManager.Instance.PotionBag.Add(item);
        potion.raycastTarget = false;
        potion.GetComponent<CanvasGroup>().alpha = 0;
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }
}
