using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;

public class CardItem
    : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IDragHandler,
        IEndDragHandler,
        IPointerDownHandler
{
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

    private CardItem rightCard,
        leftCard;
    private bool isAttackCard;
    private Enemy enemy;
    public int CardID { get; set; }
    public int Cost { get; set; }
    public RectTransform CardRectTransform { get; set; }
    public bool CantMove { get; set; }
    private Outline outline;
    public Image CardImage
    {
        get { return cardImage; }
        set { cardImage = value; }
    }
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
        SetCardInfo();
        EventManager.Instance.AddEventRegister(EventDefinition.eventRefreshUI, EventRefreshUI);
    }
    private void OnDisable()
    {
        EventManager.Instance.RemoveEventRegister(EventDefinition.eventRefreshUI, EventRefreshUI);
    }

    private void SetCardInfo()
    {
        Dictionary<int, CardData> cardList = DataManager.Instance.CardList;
        CardName.text = cardList[CardID].CardName;
        CardDescription.text = cardList[CardID].CardDescription;
        CardCost.text = cardList[CardID].CardCost.ToString();
        CardRectTransform = transform.GetComponent<RectTransform>();
        Cost = DataManager.Instance.CardList[CardID].CardCost;
        CardImage.sprite = Resources.Load<Sprite>(
           DataManager.Instance.CardList[CardID].CardImagePath
       );
        outline = GetComponentInChildren<Outline>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (BattleManager.Instance.IsDrag || CantMove)
            return;
        index = transform.GetSiblingIndex();
        Quaternion zeroRotation = Quaternion.Euler(0, 0, 0);
        transform.DOScale(2.5f, moveTime);
        transform.DORotateQuaternion(zeroRotation, moveTime);
        CardRectTransform.DOAnchorPosY(pointerEnterUpY, moveTime);
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
        CardRectTransform.DOAnchorPos(BattleManager.Instance.CardPositionList[index], moveTime);
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
                CardRectTransform.anchoredPosition,
                dragPosition
            );
            CheckRayToEnemy(false);
            return;
        }
        CardRectTransform.anchoredPosition = dragPosition;
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
                CardRectTransform.anchoredPosition,
                CardRectTransform.anchoredPosition
            );
            CheckRayToEnemy(true);
            return;
        }
        if (CardRectTransform.anchoredPosition.y >= 540 && GetUseCardCondition())
            UseCard("Player");
        else
        {
            CardRectTransform.anchoredPosition = BattleManager.Instance.CardPositionList[index];
            CardRectTransform.SetSiblingIndex(index);
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
            if (onEnd && GetUseCardCondition() && BattleManager.Instance.GetDistance(enemy.EnemyLocation)
            <= DataManager.Instance.CardList[CardID].CardAttackDistance)
            {
                enemy.OnUnSelect();
                UseCard(enemy.EnemyLocation);
            }
        }
        else if (enemy != null)
            enemy.OnUnSelect();
    }
    private bool GetUseCardCondition()
    {
        PlayerData playerData = DataManager.Instance.PlayerList[DataManager.Instance.PlayerID];
        return playerData.CurrentActionPoint >= Cost && playerData.Mana >= DataManager.Instance.CardList[CardID].CardManaCost;
    }
    //攻擊
    private void UseCard(string target)
    {
        if (BattleManager.Instance.MyBattleType != BattleManager.BattleType.Attack)
            return;
        if (BattleManager.Instance.CurrentNegativeState.Contains(BattleManager.NegativeState.CantMove)
        && DataManager.Instance.CardList[CardID].CardType == "移動")
            return;
        CardRectTransform.DOScale(1.5f, 0);
        DataManager.Instance.HandCard.Remove(this);
        BattleManager.Instance.ConsumeActionPoint(Cost);
        BattleManager.Instance.ConsumeMana(DataManager.Instance.CardList[CardID].CardManaCost);
        BattleManager.Instance.GetShield(
            DataManager.Instance.PlayerList[DataManager.Instance.PlayerID],
            DataManager.Instance.CardList[CardID].CardShield
        );
        if (DataManager.Instance.CardList[CardID].CardAttack != 0)
            BattleManager.Instance.TakeDamage(
                BattleManager.Instance.CurrentEnemyList[target],
                DataManager.Instance.CardList[CardID].CardAttack,
                target
            );
        EventManager.Instance.DispatchEvent(EventDefinition.eventUseCard, this);
        for (int i = 0; i < DataManager.Instance.CardList[CardID].CardEffectList.Count; i++)
        {
            if (DataManager.Instance.CardList[CardID].CardType == "能力")
            {
                BattleManager.Instance.CurrentAbilityList.Add(CardID, target);
                continue;
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
    private void EventRefreshUI(params object[] args)
    {
        if (DataManager.Instance.PlayerList[DataManager.Instance.PlayerID].CurrentActionPoint >= Cost
        && DataManager.Instance.PlayerList[DataManager.Instance.PlayerID].Mana >= DataManager.Instance.CardList[CardID].CardManaCost)
            outline.effectDistance = new Vector2(6, 6);
        else
            outline.effectDistance = new Vector2(0, 0);
    }
    /*private void UseCard()
    {
        if (BattleManager.Instance.MyBattleType != BattleManager.BattleType.Attack)
            return;
        CardRectTransform.DOScale(1.5f, 0);
        DataManager.Instance.HandCard.Remove(this);
        BattleManager.Instance.ConsumeActionPoint(Cost);
        BattleManager.Instance.ConsumeMana(DataManager.Instance.CardList[CardID].CardManaCost);
        BattleManager.Instance.GetShield(
            DataManager.Instance.PlayerList[DataManager.Instance.PlayerID],
            DataManager.Instance.CardList[CardID].CardShield
        );
        EventManager.Instance.DispatchEvent(EventDefinition.eventUseCard, this);
        for (int i = 0; i < DataManager.Instance.CardList[CardID].CardEffectList.Count; i++)
        {
            if (DataManager.Instance.CardList[CardID].CardType == "能力")
            {
                BattleManager.Instance.CurrentAbilityList.Add(CardID, "Player");
                return;
            }
            string effectID;
            int effectCount;
            effectID = DataManager.Instance.CardList[CardID].CardEffectList[i].Item1;
            effectCount = DataManager.Instance.CardList[CardID].CardEffectList[i].Item2;
            EffectFactory.Instance.CreateEffect(effectID).ApplyEffect(effectCount, "Player");
        }
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
        gameObject.SetActive(false);
    }*/
}
