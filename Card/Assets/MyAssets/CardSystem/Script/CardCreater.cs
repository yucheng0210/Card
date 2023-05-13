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

    [SerializeField]
    private TextAsset textAsset;
    private List<Vector2> cardPositionList = new List<Vector2>();
    private List<float> cardAngleList = new List<float>();

    private void Start()
    {
        GetExcelData(textAsset);
        CreateCard();
        StartCoroutine(DrawCard());
        EventManager.Instance.AddEventRegister(
            EventDefinition.eventUseCard,
            EventCardAdjustmentPosition
        );
    }

    private void GetExcelData(TextAsset file)
    {
        BattleManager.Instance.CardList = new List<CardData>();
        BattleManager.Instance.CardList.Clear();
        //index = 0;
        string[] lineData = file.text.Split(new char[] { '\n' });
        for (int i = 1; i < lineData.Length - 1; i++)
        {
            string[] row = lineData[i].Split(new char[] { ',' });
            if (row[1] == "")
                break;
            CardData cardData = new CardData();
            cardData.CardID = int.Parse(row[0]);
            cardData.CardName = row[1];
            cardData.CardType = row[2];
            cardData.CardImagePath = row[3];
            cardData.CardCost = int.Parse(row[4]);
            cardData.CardAttribute = row[5];
            cardData.CardEffect = row[6];
            cardData.CardDescription = row[7];
            cardData.CardAttack = int.Parse(row[8]);
            cardData.CardDefend = int.Parse(row[9]);
            cardData.CardHeld = int.Parse(row[10]);
            BattleManager.Instance.CardList.Add(cardData);
        }
    }

    private void CreateCard()
    {
        for (int i = 0; i < DataManager.Instance.CardList.Count; i++)
        {
            for (int j = DataManager.Instance.CardList[i].CardHeld; j > 0; j--)
            {
                CardItem card = Instantiate(cardPrefab, transform);
                Dictionary<int, CardData> cardList = DataManager.Instance.CardList;
                card.CardIndex = i;
                card.CardName.text = cardList[i].CardName;
                card.CardDescription.text = cardList[i].CardDescription;
                card.CardCost.text = cardList[i].CardCost.ToString();
                card.gameObject.SetActive(false);
                DataManager.Instance.CardBag.Add(card.CardIndex, card);
            }
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
        BattleManager.Instance.AddHandCard(drawCardCount);
        Dictionary<int, CardItem> handCard = DataManager.Instance.HandCard;
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
        Dictionary<int,CardItem> handCard = DataManager.Instance.HandCard;
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
}
