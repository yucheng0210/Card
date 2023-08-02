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
    public int CardID { get; set; }
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
    private float pointerEnterUpY;

    [SerializeField]
    private float pointerEnterSpacing;

    [SerializeField]
    private float pointerEnterReduceCount;

    [SerializeField]
    private float moveTime;

    public bool CantMove { get; set; }
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
            DataManager.Instance.CardList[CardID].CardImagePath
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (BattleManager.Instance.IsDrag || CantMove)
            return;
        index = transform.GetSiblingIndex();
        cost = DataManager.Instance.CardList[CardID].CardCost;
        Quaternion zeroRotation = Quaternion.Euler(0, 0, 0);
        transform.DOScale(2.5f, moveTime);
        transform.DORotateQuaternion(zeroRotation, moveTime);
        cardRectTransform.DOAnchorPosY(pointerEnterUpY, moveTime);
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
        if (BattleManager.Instance.IsDrag && !isAttackCard || CantMove)
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
        if (CantMove)
            return;
        isAttackCard = DataManager.Instance.CardList[CardID].CardType == "攻擊" ? true : false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (CantMove)
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
        if (CantMove)
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
            UseCard(DataManager.Instance.PlayerID);
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
                UseCard(enemy.EnemyLocation);
            }
        }
        else if (enemy != null)
            enemy.OnUnSelect();
    }

    private void UseCard(int target)
    {
        cardRectTransform.DOScale(1.5f, 0);
        DataManager.Instance.HandCard.Remove(this);
        BattleManager.Instance.ConsumeActionPoint(cost);
        BattleManager.Instance.GetShield(
            DataManager.Instance.PlayerList[DataManager.Instance.PlayerID],
            DataManager.Instance.CardList[CardID].CardShield
        );
        if (isAttackCard && DataManager.Instance.CardList[CardID].CardAttack != 0)
            BattleManager.Instance.TakeDamage(
                BattleManager.Instance.CurrentEnemyList[target],
                DataManager.Instance.CardList[CardID].CardAttack
            );
        EventManager.Instance.DispatchEvent(EventDefinition.eventUseCard, this);
        for (int i = 0; i < DataManager.Instance.CardList[CardID].CardEffectList.Count; i++)
        {
            if (DataManager.Instance.CardList[CardID].CardType == "能力")
            {
                BattleManager.Instance.CurrentAbilityList.Add(CardID, target);
                return;
            }
            string effectID;
            int effectCount;
            effectID = DataManager.Instance.CardList[CardID].CardEffectList[i].Item1;
            effectCount = DataManager.Instance.CardList[CardID].CardEffectList[i].Item2;
            EffectFactory.Instance.CreateEffect(effectID).ApplyEffect(effectCount, target);
        }
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
        gameObject.SetActive(false);
    }
}
