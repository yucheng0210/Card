using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class UIShop : UIBase
{
    [SerializeField]
    private Transform cardGroupTrans;
    [SerializeField]
    private Transform potionGroupTrans;
    [SerializeField]
    private Transform itemGroupTrans;
    [SerializeField]
    private Button exitButton;
    [SerializeField]
    private CardItem cardPrefab;
    protected override void Start()
    {
        base.Start();
        exitButton.onClick.AddListener(() => BattleManager.Instance.NextLevel("UIShop"));
    }
    public override void Show()
    {
        base.Show();
        RefreshMerchandise();
    }
    private void RefreshMerchandise()
    {
        Dictionary<int, CardData> cardList = DataManager.Instance.CardList;
        for (int i = 0; i < cardGroupTrans.childCount; i++)
        {
            Destroy(cardGroupTrans.GetChild(i).gameObject);
        }
        for (int i = 0; i < 6; i++)
        {
            int randomIndex = Random.Range(0, cardList.Count);
            KeyValuePair<int, CardData> randomCard = cardList.ElementAt(randomIndex);
            CardData cardData = cardList[randomCard.Key];
            CardItem cardItem = Instantiate(cardPrefab, cardGroupTrans);
            Text cardPriceText = cardItem.transform.GetChild(cardItem.transform.childCount - 1).GetComponent<Text>();
            cardItem.GetComponent<CanvasGroup>().alpha = 1;
            cardItem.CardID = randomCard.Key;
            cardItem.CantMove = true;
            cardPriceText.text = "$" + cardData.CardBuyPrice.ToString();
            Button cardButton = cardItem.GetComponent<Button>();
            cardButton.onClick.AddListener(() => AddCard(cardItem.CardID, cardButton.gameObject));
        }
    }
    private void AddCard(int cardID, GameObject card)
    {
        CardData cardData = DataManager.Instance.CardList[cardID];
        if (DataManager.Instance.MoneyCount < cardData.CardBuyPrice)
            return;
        DataManager.Instance.MoneyCount -= cardData.CardBuyPrice;
        DataManager.Instance.CardBag.Add(cardData);
        Destroy(card);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }
}
