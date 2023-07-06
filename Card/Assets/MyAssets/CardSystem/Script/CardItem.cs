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
        IDragHandler,
        IEndDragHandler,
        IPointerDownHandler
{
    public int CardIndex { get; set; }
    private int index;

    [SerializeField]
    private Text cardName;

    [SerializeField]
    private Text cardCost;

    [SerializeField]
    private Text cardDescription;

    [SerializeField]
    private Image cardImage;

    [SerializeField]
    private GameObject collision;

    [SerializeField]
    private float onPointerEnterUp;

    [SerializeField]
    private float pointerEnterSpacing;

    [SerializeField]
    private float pointerEnterReduceCount;

    [SerializeField]
    private float moveTime;

    [SerializeField]
    private bool cantMove;
    private RectTransform cardRectTransform;
    private CardItem rightCard,
        leftCard;
    private bool isAttackCard;
    private Enemy enemy;
    private int cost;
    public GameObject Collision
    {
        get { return collision; }
        set { collision = value; }
    }
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
        cardRectTransform = transform.GetComponent<RectTransform>();
        cardImage.sprite = Resources.Load<Sprite>(
            DataManager.Instance.CardList[CardIndex].CardImagePath
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (BattleManager.Instance.IsDrag || cantMove)
            return;
        index = transform.GetSiblingIndex();
        cost = DataManager.Instance.CardList[CardIndex].CardCost;
        Quaternion zeroRotation = Quaternion.Euler(0, 0, 0);
        transform.DOScale(2.5f, moveTime);
        transform.DORotateQuaternion(zeroRotation, moveTime);
        cardRectTransform.DOAnchorPosY(
            BattleManager.Instance.CardPositionList[index].y + onPointerEnterUp,
            moveTime
        );
        float space = pointerEnterSpacing;
        for (int i = index + 1; i < transform.parent.childCount; i++)
        {
            rightCard = transform.parent.GetChild(i).GetComponent<CardItem>();
            rightCard
                .GetComponent<RectTransform>()
                .DOAnchorPosX(BattleManager.Instance.CardPositionList[i].x + space, moveTime);
            space -= pointerEnterReduceCount;
            if (space <= 0)
                space = pointerEnterReduceCount;
        }
        space = pointerEnterSpacing;
        for (int i = index - 1; i >= 0; i--)
        {
            leftCard = transform.parent.GetChild(i).GetComponent<CardItem>();
            leftCard
                .GetComponent<RectTransform>()
                .DOAnchorPosX(BattleManager.Instance.CardPositionList[i].x - space, moveTime);
            space -= pointerEnterReduceCount;
            if (space <= 0)
                space = pointerEnterReduceCount;
        }
        transform.SetAsLastSibling();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (BattleManager.Instance.IsDrag && !isAttackCard || cantMove)
            return;
        transform.DOScale(1.5f, moveTime);
        transform.DORotateQuaternion(
            Quaternion.Euler(0, 0, BattleManager.Instance.CardAngleList[index]),
            moveTime
        );
        cardRectTransform.DOAnchorPos(BattleManager.Instance.CardPositionList[index], moveTime);
        transform.SetSiblingIndex(index);
        for (int i = index + 1; i < transform.parent.childCount; i++)
        {
            rightCard = transform.parent.GetChild(i).GetComponent<CardItem>();
            rightCard
                .GetComponent<RectTransform>()
                .DOAnchorPosX(BattleManager.Instance.CardPositionList[i].x, moveTime);
        }
        for (int i = index - 1; i >= 0; i--)
        {
            leftCard = transform.parent.GetChild(i).GetComponent<CardItem>();
            leftCard
                .GetComponent<RectTransform>()
                .DOAnchorPosX(BattleManager.Instance.CardPositionList[i].x, moveTime);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (cantMove)
            return;
        isAttackCard = DataManager.Instance.CardList[CardIndex].CardType == "攻擊" ? true : false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (cantMove)
            return;
        BattleManager.Instance.IsDrag = true;
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
        if (cantMove)
            return;
        BattleManager.Instance.IsDrag = false;
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
            cardRectTransform.anchoredPosition = BattleManager.Instance.CardPositionList[index];
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
        cardRectTransform.DOScale(1.5f, 0);
        DataManager.Instance.HandCard.Remove(this);
        EventManager.Instance.DispatchEvent(EventDefinition.eventUseCard, this);
        BattleManager.Instance.ConsumeActionPoint(cost);
        if (isAttackCard && DataManager.Instance.CardList[CardIndex].CardAttack != 0)
            BattleManager.Instance.TakeDamage(
                DataManager.Instance.EnemyList[1001],
                DataManager.Instance.CardList[CardIndex].CardAttack
            );
        BattleManager.Instance.GetShield(
            DataManager.Instance.PlayerList[DataManager.Instance.PlayerID],
            DataManager.Instance.CardList[CardIndex].CardShield
        );
        gameObject.SetActive(false);
        for (int i = 0; i < DataManager.Instance.CardList[CardIndex].CardEffectList.Count; i++)
        {
            string effectID;
            int effectCount;
            effectID = DataManager.Instance.CardList[CardIndex].CardEffectList[i].Item1;
            effectCount = DataManager.Instance.CardList[CardIndex].CardEffectList[i].Item2;
            BattleManager.Instance.CardEffectFactory
                .CreateEffect(effectID, effectCount)
                .ApplyEffect();
        }
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }
}
