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
    private RectTransform rightCard,
        leftCard,
        cardRectTransform;
    private Quaternion initialRotation;
    private Vector2 initialPosition;
    private bool isAttackCard;
    private Enemy enemy;
    private int cost;

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
        cost = DataManager.Instance.CardList[CardIndex].CardCost;
        //index = BattleManager.Instance.HandCard.IndexOf(this);
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
        isAttackCard = DataManager.Instance.CardList[CardIndex].CardType == "攻擊" ? true : false;
        if (isAttackCard)
        {
            EventManager.Instance.DispatchEvent(
                EventDefinition.eventAttackLine,
                true,
                cardRectTransform.anchoredPosition,
                cardRectTransform.anchoredPosition
            );
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
        Cursor.visible = false;
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
            EventManager.Instance.DispatchEvent(
                EventDefinition.eventAttackLine,
                true,
                cardRectTransform.anchoredPosition,
                dragPosition
            );
            CheckRayToEnemy(false);
            return;
        }
        cardRectTransform.anchoredPosition = dragPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Cursor.visible = true;
        if (isAttackCard)
        {
            EventManager.Instance.DispatchEvent(
                EventDefinition.eventAttackLine,
                false,
                cardRectTransform.anchoredPosition,
                cardRectTransform.anchoredPosition
            );
            CheckRayToEnemy(true);
            return;
        }
        if (
            cardRectTransform.anchoredPosition.y >= 540
            && DataManager.Instance.PlayerList[DataManager.Instance.PlayerID].CurrentActionPoint
                >= cost
        )
            UseCard();
        else
        {
            cardRectTransform.anchoredPosition = initialPosition;
            cardRectTransform.SetSiblingIndex(index);
        }
    }

    private void CheckRayToEnemy(bool onEnd)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10000, LayerMask.GetMask("Enemy")))
        {
            enemy = hit.transform.GetComponent<Enemy>();
            enemy.OnSelect();
            if (
                onEnd
                && DataManager.Instance.PlayerList[DataManager.Instance.PlayerID].CurrentActionPoint
                    >= cost
            )
            {
                enemy.OnUnSelect();
                UseCard();
            }
        }
        else if (enemy != null)
            enemy.OnUnSelect();
    }

    private void UseCard()
    {
        DataManager.Instance.HandCard.Remove(this);
        EventManager.Instance.DispatchEvent(EventDefinition.eventUseCard, this);
        BattleManager.Instance.ConsumeActionPoint(cost);
        BattleManager.Instance.TakeDamage(
            DataManager.Instance.EnemyList[1001],
            DataManager.Instance.CardList[CardIndex].CardAttack
        );
        BattleManager.Instance.GetShield(
            DataManager.Instance.PlayerList[DataManager.Instance.PlayerID],
            DataManager.Instance.CardList[CardIndex].CardShield
        );
        gameObject.SetActive(false);
    }
}
