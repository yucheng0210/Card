using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardCreater : MonoBehaviour
{
    [SerializeField]
    private CardItem cardPrefab;

    [SerializeField]
    private Transform handCardTrans;

    [SerializeField]
    private Transform usedCardTrans;

    [SerializeField]
    private int drawCardCount;

    [SerializeField]
    private float coolDown;

    [SerializeField]
    private float cardXSpacing;

    [SerializeField]
    private float cardYSpacing;

    [SerializeField]
    private float reduceValue;

    [SerializeField]
    private float minCardAngle;

    [SerializeField]
    private GameObject roundTip;

    private List<Vector2> cardPositionList = new List<Vector2>();
    private List<float> cardAngleList = new List<float>();
    private List<CardItem> cardItemList = new List<CardItem>();

    private void Start()
    {
        CreateCard();
        StartCoroutine(DrawCard());
        EventManager.Instance.AddEventRegister(
            EventDefinition.eventUseCard,
            EventCardAdjustmentPosition
        );
        EventManager.Instance.AddEventRegister(EventDefinition.eventEnemyTurn, EventEnemyTurn);
    }

    private void CreateCard()
    {
        List<CardData> cardBag = DataManager.Instance.CardBag;
        for (int i = 0; i < cardBag.Count; i++)
        {
            CardItem cardItem = Instantiate(cardPrefab, transform);
            cardItem.CardIndex = cardBag[i].CardID;
            cardItem.CardName.text = cardBag[i].CardName;
            cardItem.CardDescription.text = cardBag[i].CardDescription;
            cardItem.CardCost.text = cardBag[i].CardCost.ToString();
            cardItem.gameObject.SetActive(false);
            cardItemList.Add(cardItem);
        }
    }

    private void CalculatePositionAngle(int cardCount)
    {
        Vector2 startPosition = new Vector2(878, -50);
        int odd = cardCount % 2 != 0 ? 0 : 1;
        float startAngle = (cardCount / 2 - odd) * minCardAngle;
        for (int i = 0; i < cardCount; i++)
        {
            cardPositionList.Add(startPosition);
            cardAngleList.Add(startAngle);
            for (int j = i; j > 0; j--)
            {
                float adjustmentPosX = startPosition.x;
                adjustmentPosX -= (i - j + 1) * cardXSpacing * 2;
                cardPositionList[j - 1] = new Vector2(adjustmentPosX, cardPositionList[j - 1].y);
            }
            startPosition.x += cardXSpacing;
            if (odd != 0 && i == cardCount / 2 - 1)
                continue;
            startAngle -= minCardAngle;
            startPosition.y += cardYSpacing * (cardCount / 2 - odd - i);
        }
    }

    private IEnumerator DrawCard()
    {
        BattleManager.Instance.Shuffle();
        StartCoroutine(UIManager.Instance.FadeOutIn(roundTip.GetComponent<CanvasGroup>(), 0.5f, 1));
        yield return new WaitForSecondsRealtime(1.5f);
        Vector2 startPosition = new Vector2(878, -50);
        int odd = drawCardCount % 2 != 0 ? 0 : 1;
        float startAngle = (drawCardCount / 2 - odd) * minCardAngle;
        BattleManager.Instance.AddHandCard(drawCardCount, cardItemList);
        List<CardItem> handCard = DataManager.Instance.HandCard;
        for (int i = 0; i < handCard.Count; i++)
        {
            yield return new WaitForSecondsRealtime(coolDown);
            handCard[i].transform.SetParent(handCardTrans);
            handCard[i].gameObject.SetActive(true);
            handCard[i].GetComponent<RectTransform>().DOAnchorPos(startPosition, 0.5f);
            handCard[i].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, startAngle);
            for (int j = i; j > 0; j--)
            {
                handCard[j - 1]
                    .GetComponent<RectTransform>()
                    .DOAnchorPosX(startPosition.x - (i - j + 1) * cardXSpacing * 2, 0.5f);
            }
            startPosition.x += cardXSpacing;
            if (odd != 0 && i == drawCardCount / 2 - 1)
                continue;
            startAngle -= minCardAngle;
            startPosition.y += cardYSpacing * (drawCardCount / 2 - odd - i);
        }
    }

    private void EventCardAdjustmentPosition(params object[] args)
    {
        cardPositionList.Clear();
        cardAngleList.Clear();
        DataManager.Instance.UsedCardBag.Add(
            DataManager.Instance.CardList[((CardItem)args[0]).CardIndex]
        );
        ((CardItem)args[0]).transform.SetParent(usedCardTrans);
        cardItemList.Remove((CardItem)args[0]);
        List<CardItem> handCard = DataManager.Instance.HandCard;
        CalculatePositionAngle(handCard.Count);
        for (int i = 0; i < handCard.Count; i++)
        {
            handCard[i].GetComponent<RectTransform>().DOAnchorPos(cardPositionList[i], 0.5f);
            handCard[i].GetComponent<RectTransform>().rotation = Quaternion.Euler(
                0,
                0,
                cardAngleList[i]
            );
        }
    }

    private void EventEnemyTurn(params object[] args)
    {
        for (int i = 0; i < DataManager.Instance.HandCard.Count; i++)
        {
            DataManager.Instance.UsedCardBag.Add(
                DataManager.Instance.CardList[DataManager.Instance.HandCard[i].CardIndex]
            );
        }
        DataManager.Instance.HandCard.Clear();
    }
}
