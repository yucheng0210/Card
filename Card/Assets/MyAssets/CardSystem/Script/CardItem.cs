using System;
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
        IEndDragHandler,
        IPointerDownHandler
{
    public int CardIndex { get; set; }
    private int index;

    [SerializeField]
    private float pointerEnterSpacing;

    [SerializeField]
    private Text cardName;

    [SerializeField]
    private Text cardCost;

    [SerializeField]
    private Text cardDescription;

    [SerializeField]
    private GameObject attackLine;
    private RectTransform rightCard,
        leftCard,
        cardRectTransform;
    private Quaternion initialRotation;
    private Vector2 initialPosition;
    private bool isAttackCard;
    private bool isUseLine;

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
        index = BattleManager.Instance.HandCard.IndexOf(this);
        Quaternion zeroRotation = Quaternion.Euler(0, 0, 0);
        initialRotation = transform.rotation;
        transform.DOScale(2.5f, 0.25f);
        transform.DORotateQuaternion(zeroRotation, 0.25f);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        //index = transform.GetSiblingIndex();
        // transform.SetAsLastSibling();
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
        //transform.SetSiblingIndex(index);
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

    public void OnPointerDown(PointerEventData eventData)
    {
        isAttackCard = BattleManager.Instance.CardList[CardIndex].CardType == "攻擊" ? true : false;
        if (isAttackCard)
        {
            ((UIAttackLine)UIManager.Instance.FindUI("UIAttackLine")).SetStartPos(
                cardRectTransform.anchoredPosition
            );
            UIManager.Instance.ShowAttackLine(true);
            return;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isAttackCard)
            return;
        initialPosition = cardRectTransform.anchoredPosition;
        initialRotation = transform.rotation;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 dragPosition;
        if (
            !RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform.parent.GetComponent<RectTransform>(),
                eventData.position,
                eventData.pressEventCamera,
                out dragPosition
            )
        )
            return;
        if (isAttackCard)
        {
            ((UIAttackLine)UIManager.Instance.FindUI("UIAttackLine")).SetEndPos(dragPosition);
            return;
        }
        cardRectTransform.anchoredPosition = dragPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isAttackCard)
        {
            isAttackCard = false;
            UIManager.Instance.ShowAttackLine(false);
            return;
        }
        if (cardRectTransform.anchoredPosition.y >= 540)
            UseCard();
        else
        {
            cardRectTransform.anchoredPosition = initialPosition;
            transform.SetSiblingIndex(index);
        }
    }

    private void UseCard()
    {
        int cost = BattleManager.Instance.CardList[CardIndex].CardCost;
        BattleManager.Instance.HandCard.RemoveAt(index);
        EventManager.Instance.DispatchEvent(EventDefinition.eventUseCard);
        BattleManager.Instance.ConsumeActionPoint(cost);
        BattleManager.Instance.GetShield(BattleManager.Instance.CardList[CardIndex].CardDefend);
        Destroy(gameObject);
    }
}
