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

    public List<Card> CardList { get; private set; }

    [SerializeField]
    private TextAsset textAsset;

    private List<GameObject> cardBag = new List<GameObject>();
    private Card cardData;

    private void Start()
    {
        GetTextFromFile(textAsset);
        CreateCard();
        StartCoroutine(CardPositionAdjustment());
    }

    private void CreateCard()
    {
        for (int i = 0; i < CardList.Count; i++)
        {
            for (int j = CardList[i].cardHeld; j > 0; j--)
            {
                CardItem card = Instantiate(cardPrefab, transform);
                card.CardName.text = CardList[i].CardName;
                card.CardDescription.text = CardList[i].CardDescription;
                card.CardCost.text = CardList[i].CardCost.ToString();
                card.gameObject.SetActive(false);
                cardBag.Add(card.gameObject);
            }
        }
    }

    private void Shuffle()
    {
        for (int i = cardBag.Count - 1; i >= 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, cardBag.Count);
            GameObject temp = cardBag[randomIndex];
            cardBag[randomIndex] = cardBag[i];
            cardBag[i] = temp;
        }
    }

    private void GetTextFromFile(TextAsset file)
    {
        CardList = new List<Card>();
        CardList.Clear();
        //index = 0;
        string[] lineData = file.text.Split(new char[] { '\n' });
        for (int i = 1; i < lineData.Length - 1; i++)
        {
            string[] row = lineData[i].Split(new char[] { ',' });
            if (row[1] == "")
                break;
            cardData = new Card();
            cardData.CardID = int.Parse(row[0]);
            cardData.CardName = row[1];
            cardData.CardType = row[2];
            cardData.CardImagePath = row[3];
            cardData.CardCost = int.Parse(row[4]);
            cardData.CardAttribute = row[5];
            cardData.CardEffect = row[6];
            cardData.CardDescription = row[7];
            cardData.cardHeld = int.Parse(row[8]);
            CardList.Add(cardData);
        }
    }

    private IEnumerator CardPositionAdjustment()
    {
        Shuffle();
        StartCoroutine(UIManager.Instance.FadeOutIn(roundTip.GetComponent<CanvasGroup>(), 0.5f, 1));
        yield return new WaitForSecondsRealtime(1.5f);
        Vector2 startPosition = new Vector2(878, -50);
        int odd = drawCardCount % 2 != 0 ? 0 : 1;
        float startAngle = (drawCardCount / 2 - odd) * minCardAngle;
        for (int i = 0; i < drawCardCount; i++)
        {
            /* if (i > maxCard - 1)
                 cardXSpacing -= reduceValue;*/
            cardBag[i].transform.SetParent(handCardTrans);
            cardBag[i].SetActive(true);
            cardBag[i].GetComponent<RectTransform>().DOAnchorPos(startPosition, 0.5f);
            cardBag[i].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, startAngle);
            for (int j = i; j > 0; j--)
            {
                cardBag[j - 1]
                    .GetComponent<RectTransform>()
                    .DOAnchorPosX(startPosition.x - (i - j + 1) * cardXSpacing * 2, 0.5f);
            }
            startPosition.x += cardXSpacing;
            if (odd != 0 && i == drawCardCount / 2 - 1)
            {
                yield return new WaitForSecondsRealtime(coolDown);
                continue;
            }
            startAngle -= minCardAngle;
            startPosition.y += cardYSpacing * (drawCardCount / 2 - odd - i);
            yield return new WaitForSecondsRealtime(coolDown);
        }
    }
}
