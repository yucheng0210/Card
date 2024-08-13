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
        for (int i = 0; i < cardGroupTrans.childCount; i++)
        {
            Destroy(cardGroupTrans.GetChild(i).gameObject);
        }
        for (int i = 0; i < 6; i++)
        {
            int randomIndex = Random.Range(0, DataManager.Instance.CardList.Count);
            CardItem cardItem = Instantiate(BattleManager.Instance.CardPrefab, cardGroupTrans);
            cardItem.GetComponent<CanvasGroup>().alpha = 1;
            cardItem.CardID = DataManager.Instance.CardList.ElementAt(randomIndex).Key;
            cardItem.CantMove = true;
            Button cardButton = cardItem.gameObject.AddComponent<Button>();
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
