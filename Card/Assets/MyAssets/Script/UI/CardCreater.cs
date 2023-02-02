using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardCreater : MonoBehaviour
{
    [SerializeField]
    private GameObject cardPrefab;

    [SerializeField]
    private int maxCard;
    private List<GameObject> cardList = new List<GameObject>();
    private List<Vector2> cardPositionList;
    private float cardWidth;

    private void Start()
    {
        CreateCard();
        StartCoroutine(CardPositionAdjustment());
    }

    private void CreateCard()
    {
        for (int i = 0; i < maxCard; i++)
        {
            GameObject card = Instantiate(cardPrefab, transform);
            cardList.Add(card);
        }
    }

    private IEnumerator CardPositionAdjustment()
    {
        cardPositionList = new List<Vector2>();
        float cardOffset = 160;
        Vector2 startPosition = new Vector2(
            (cardOffset * (int)(cardList.Count / 2) + cardOffset / 2),
            0
        );
        for (int i = 0; i < cardList.Count; i++)
        {
            cardList[i].GetComponent<RectTransform>().DOAnchorPos(startPosition, 0.5f);
            startPosition.x += cardOffset;
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }
}
