using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class CardItem
    : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
{
    private int index;

    [SerializeField]
    private float pointerEnterSpacing;

    private RectTransform rightCard,
        leftCard,
        cardRectTransform;
    private Quaternion initialRotation;
    private Vector2 initialPosition;

    [SerializeField]
    private Text cardName;

    [SerializeField]
    private Text cardCost;

    [SerializeField]
    private Text cardDescription;
    public Text CardName
    {
        get { return cardName; }
        set { cardName = value; }
    }
    public Text CardCost
    {
        get { return cardCost; }
        set { cardCost = value; }
    }
    public Text CardDescription
    {
        get { return cardDescription; }
        set { cardDescription = value; }
    }

    private void Start()
    {
        initialPosition = transform.GetComponent<RectTransform>().anchoredPosition;
        cardRectTransform = transform.GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Quaternion zeroRotation = Quaternion.Euler(0, 0, 0);
        initialRotation = transform.rotation;
        transform.DOScale(2.5f, 0.25f);
        transform.DORotateQuaternion(zeroRotation, 0.25f);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        index = transform.GetSiblingIndex();
        transform.SetAsLastSibling();
        /*for (int i = index + 1; i < transform.parent.childCount; i++)
        {
            rightCard = transform.parent.GetChild(i).GetComponent<RectTransform>();
            rightCard.DOAnchorPosX(rightCard.anchoredPosition.x + pointerEnterSpacing, 0.25f);
        }
        for (int i = index - 1; i >= 0; i--)
        {
            leftCard = transform.parent.GetChild(i).GetComponent<RectTransform>();
            leftCard.DOAnchorPosX(leftCard.anchoredPosition.x - pointerEnterSpacing, 0.25f);
        }*/
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(1.5f, 0.25f);
        transform.DORotateQuaternion(initialRotation, 0.25f);
        transform.SetSiblingIndex(index);
        /*for (int i = index + 1; i < transform.parent.childCount; i++)
        {
            rightCard = transform.parent.GetChild(i).GetComponent<RectTransform>();
            rightCard.DOAnchorPosX(rightCard.anchoredPosition.x - pointerEnterSpacing, 0.25f);
        }
        for (int i = index - 1; i >= 0; i--)
        {
            leftCard = transform.parent.GetChild(i).GetComponent<RectTransform>();
            leftCard.DOAnchorPosX(leftCard.anchoredPosition.x + pointerEnterSpacing, 0.25f);
        }*/
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        initialPosition = cardRectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 dragPosition;
        if (
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform.parent.GetComponent<RectTransform>(),
                eventData.position,
                eventData.pressEventCamera,
                out dragPosition
            )
        )
            cardRectTransform.anchoredPosition = dragPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        cardRectTransform.anchoredPosition = initialPosition;
        transform.SetSiblingIndex(index);
    }
}
