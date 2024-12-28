using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
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
            RefreshMerchandise_SkyIsland();
        }
        else
        {
            RefreshMerchandise_Battle();
        }
    }
    private void RefreshMerchandise_Battle()
    {
        Dictionary<int, CardData> cardList = DataManager.Instance.CardList;
        Dictionary<int, Item> itemList = DataManager.Instance.ItemList;
        Dictionary<int, Potion> potionList = DataManager.Instance.PotionList;
        battleBackground.SetActive(true);
        skyIslandBackground.SetActive(false);
        for (int i = 0; i < cardGroupTrans.childCount; i++)
        {
            Destroy(cardGroupTrans.GetChild(i).gameObject);
        }
        for (int i = 0; i < 6; i++)
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
            Text cardPriceText = cardItem.transform.GetChild(cardItem.transform.childCount - 1).GetComponent<Text>();
            cardItem.GetComponent<CanvasGroup>().alpha = 1;
            cardItem.MyCardData = cardData;
            cardItem.MyCardData.CardID = randomCard.Key;
            cardItem.CantMove = true;
            cardPriceText.text = cardData.CardBuyPrice.ToString();
            Button cardButton = cardItem.CardImage.GetComponent<Button>();
            cardButton.onClick.AddListener(() => AddCard(cardItem.MyCardData.CardID, cardItem.CardImage));
        }
        for (int i = 0; i < potionGroupTrans.childCount; i++)
        {
            Destroy(potionGroupTrans.GetChild(i).gameObject);
        }
        for (int i = 0; i < 4; i++)
        {
            int randomIndex = Random.Range(0, potionList.Count);
            KeyValuePair<int, Potion> randomItem = potionList.ElementAt(randomIndex);
            Potion potionItem = potionList[randomItem.Key];
            Button potion = Instantiate(potionPrefab, potionGroupTrans);
            potion.GetComponent<Image>().sprite = Resources.Load<Sprite>(potionItem.ItemImagePath);
            Text potionPriceText = potion.transform.GetChild(potion.transform.childCount - 1).GetComponent<Text>();
            potionPriceText.text = potionItem.ItemBuyPrice.ToString();
            potion.onClick.AddListener(() => AddPotion(potionItem.ItemID, potion.GetComponent<Image>()));
        }
    }
    private void RefreshMerchandise_SkyIsland()
    {
        Dictionary<int, CardData> cardList = DataManager.Instance.CardList;
        Dictionary<int, Item> itemList = DataManager.Instance.ItemList;
        Dictionary<int, Potion> potionList = DataManager.Instance.PotionList;
        battleBackground.SetActive(false);
        skyIslandBackground.SetActive(true);
        for (int i = 0; i < cardGroupTrans.childCount; i++)
        {
            Destroy(cardGroupTrans.GetChild(i).gameObject);
        }
        for (int i = 0; i < 6; i++)
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
            Text cardPriceText = cardItem.transform.GetChild(cardItem.transform.childCount - 1).GetComponent<Text>();
            cardItem.GetComponent<CanvasGroup>().alpha = 1;
            cardItem.MyCardData = cardData;
            cardItem.MyCardData.CardID = randomCard.Key;
            cardItem.CantMove = true;
            cardPriceText.text = cardData.CardBuyPrice.ToString();
            Button cardButton = cardItem.CardImage.GetComponent<Button>();
            cardButton.onClick.AddListener(() => AddCard(cardItem.MyCardData.CardID, cardItem.CardImage));
        }
        for (int i = 0; i < potionGroupTrans.childCount; i++)
        {
            Destroy(potionGroupTrans.GetChild(i).gameObject);
        }
        for (int i = 0; i < 2; i++)
        {
            int randomIndex = Random.Range(0, potionList.Count);
            KeyValuePair<int, Potion> randomItem = potionList.ElementAt(randomIndex);
            Potion potionItem = potionList[randomItem.Key];
            Button potion = Instantiate(potionPrefab, potionGroupTrans);
            potion.GetComponent<Image>().sprite = Resources.Load<Sprite>(potionItem.ItemImagePath);
            Text potionPriceText = potion.transform.GetChild(potion.transform.childCount - 1).GetComponent<Text>();
            potionPriceText.text = potionItem.ItemBuyPrice.ToString();
            potion.onClick.AddListener(() => AddPotion(potionItem.ItemID, potion.GetComponent<Image>()));
        }
        for (int i = 0; i < 2; i++)
        {
            int randomIndex = Random.Range(0, itemList.Count);
            KeyValuePair<int, Item> randomItem = itemList.ElementAt(randomIndex);
            Item potionItem = itemList[randomItem.Key];
            Button potion = Instantiate(potionPrefab, potionGroupTrans);
            potion.GetComponent<Image>().sprite = Resources.Load<Sprite>(potionItem.ItemImagePath);
            Text potionPriceText = potion.transform.GetChild(potion.transform.childCount - 1).GetComponent<Text>();
            potionPriceText.text = potionItem.ItemBuyPrice.ToString();
            potion.onClick.AddListener(() => AddItem(potionItem.ItemID, potion.GetComponent<Image>()));
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
    private void AddPotion(int potionID, Image potion)
    {
        Potion item = DataManager.Instance.PotionList[potionID];
        if (DataManager.Instance.MoneyCount < item.ItemBuyPrice)
        {
            return;
        }
        DataManager.Instance.MoneyCount -= item.ItemBuyPrice;
        DataManager.Instance.PotionBag.Add(item);
        potion.raycastTarget = false;
        potion.GetComponent<CanvasGroup>().alpha = 0;
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }
}
