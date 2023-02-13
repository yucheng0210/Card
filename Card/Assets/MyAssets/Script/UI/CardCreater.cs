using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardCreater : MonoBehaviour
{
    [SerializeField]
    private GameObject cardPrefab;

    [SerializeField]
    private int cardCount;

    [SerializeField]
    private int maxCard;

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

    private List<GameObject> cardList = new List<GameObject>();

    private void Start()
    {
        CreateCard();
        StartCoroutine(CardPositionAdjustment());
    }

    private void CreateCard()
    {
        for (int i = 0; i < cardCount; i++)
        {
            GameObject card = Instantiate(cardPrefab, transform);
            card.SetActive(false);
            cardList.Add(card);
        }
    }

    private IEnumerator CardPositionAdjustment()
    {
        StartCoroutine(UIManager.Instance.FadeOutIn(roundTip.GetComponent<CanvasGroup>(), 0.5f, 1));
        yield return new WaitForSecondsRealtime(1.5f);
        Vector2 startPosition = new Vector2(878, -50);
        int odd = cardList.Count % 2 != 0 ? 0 : 1;
        float startAngle = (cardList.Count / 2 - odd) * minCardAngle;
        for (int i = 0; i < cardList.Count; i++)
        {
            /* if (i > maxCard - 1)
                 cardXSpacing -= reduceValue;*/
            cardList[i].SetActive(true);
            cardList[i].GetComponent<RectTransform>().DOAnchorPos(startPosition, 0.5f);
            cardList[i].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, startAngle);
            for (int j = i; j > 0; j--)
            {
                cardList[j - 1]
                    .GetComponent<RectTransform>()
                    .DOAnchorPosX(startPosition.x - (i - j + 1) * cardXSpacing * 2, 0.5f);
            }
            startPosition.x += cardXSpacing;
            if (odd != 0 && i == cardList.Count / 2 - 1)
            {
                yield return new WaitForSecondsRealtime(coolDown);
                continue;
            }
            startAngle -= minCardAngle;
            startPosition.y += cardYSpacing * (cardList.Count / 2 - odd - i);
            yield return new WaitForSecondsRealtime(coolDown);
        }
    }
}
