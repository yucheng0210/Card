using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

using PilotoStudio;
using Unity.VisualScripting;

public class CardItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerClickHandler
{
    private int index;

    [SerializeField]
    private Text cardName;

    [SerializeField]
    private Text cardCost;
    [SerializeField]
    private Text manaCost;
    [SerializeField]
    private Text cardDescription;

    [SerializeField]
    private Image cardImage;
    [SerializeField]
    private GameObject cardOutline;

    [SerializeField]
    private float pointerEnterUpY;

    [SerializeField]
    private float pointerEnterSpacing;

    [SerializeField]
    private float pointerEnterReduceCount;

    [SerializeField]
    private float moveTime;
    private CardItem rightCard, leftCard;
    private bool isAttackCard;
    private Enemy enemy;
    //public int CardID { get; set; }
    public CardData MyCardData { get; set; }
    //public int Cost { get; set; }
    public RectTransform CardRectTransform { get; set; }
    public bool CantMove { get; set; }
    public Vector2 CurrentPos { get; set; }
    public float CurrentAngle { get; set; }
    private bool isDrag;
    public Image CardImage
    {
        get { return cardImage; }
        set { cardImage = value; }
    }
    public GameObject CardOutline
    {
        get { return cardOutline; }
        set { cardOutline = value; }
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
    public Text CardManaCost
    {
        get { return manaCost; }
        set { manaCost = value; }
    }
    public Text CardDescription
    {
        get { return cardDescription; }
        set { cardDescription = value; }
    }
    private void Start()
    {
        SetCardInfo();
        EventManager.Instance.AddEventRegister(EventDefinition.eventUseCard, RefreshCardOutline);
        EventManager.Instance.AddEventRegister(EventDefinition.eventReloadGame, EventReloadGame);
    }
    private void OnDisable()
    {
        EventManager.Instance.RemoveEventRegister(EventDefinition.eventUseCard, RefreshCardOutline);
        EventManager.Instance.RemoveEventRegister(EventDefinition.eventReloadGame, EventReloadGame);
    }
    private void SetCardInfo()
    {
        CardName.text = MyCardData.CardName;
        CardDescription.text = MyCardData.CardDescription;
        CardRectTransform = transform.GetComponent<RectTransform>();
        CardImage.sprite = Resources.Load<Sprite>(MyCardData.CardImagePath);
        if (MyCardData.CardCost >= 0)
        {
            CardCost.text = MyCardData.CardCost >= 0 ? MyCardData.CardCost.ToString() : "";
            CardManaCost.text = MyCardData.CardManaCost.ToString();
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDrag || CantMove || BattleManager.Instance.MyBattleType != BattleManager.BattleType.Attack)
        {
            return;
        }
        index = transform.GetSiblingIndex();
        Quaternion zeroRotation = Quaternion.Euler(0, 0, 0);
        transform.DOScale(1.5f, moveTime);
        transform.DORotateQuaternion(zeroRotation, moveTime);
        CardRectTransform.DOAnchorPosY(pointerEnterUpY, moveTime);
        float space = pointerEnterSpacing;
        for (int i = index + 1; i < transform.parent.childCount; i++)
        {
            rightCard = transform.parent.GetChild(i).GetComponent<CardItem>();
            rightCard.GetComponent<RectTransform>().DOAnchorPosX(rightCard.CurrentPos.x + space, moveTime);
            space -= pointerEnterReduceCount;
            if (space <= 0)
            {
                space = pointerEnterReduceCount;
            }
        }
        space = pointerEnterSpacing;
        for (int i = index - 1; i >= 0; i--)
        {
            leftCard = transform.parent.GetChild(i).GetComponent<CardItem>();
            leftCard.GetComponent<RectTransform>().DOAnchorPosX(leftCard.CurrentPos.x - space, moveTime);
            space -= pointerEnterReduceCount;
            if (space <= 0)
            {
                space = pointerEnterReduceCount;
            }
        }
        transform.SetAsLastSibling();
        string location = BattleManager.Instance.CurrentPlayerLocation;
        int cardAttackDistance = MyCardData.CardAttackDistance;
        BattleManager.CheckEmptyType checkEmptyType = BattleManager.CheckEmptyType.PlayerAttack;
        BattleManager.ActionRangeType actionRangeType = BattleManager.ActionRangeType.Default;
        List<string> emptyPlaceList = BattleManager.Instance.GetActionRangeTypeList(location, cardAttackDistance, checkEmptyType, actionRangeType);
        UIManager.Instance.ChangeCheckerboardColor(emptyPlaceList, false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDrag || CantMove || BattleManager.Instance.MyBattleType != BattleManager.BattleType.Attack)
        {
            return;
        }
        ResetCardPos();
    }
    private void ResetCardPos()
    {
        transform.DOScale(1f, moveTime);
        transform.DORotateQuaternion(Quaternion.Euler(0, 0, CurrentAngle), moveTime);
        CardRectTransform.DOAnchorPos(CurrentPos, moveTime);
        transform.SetSiblingIndex(index);
        for (int i = index + 1; i < transform.parent.childCount; i++)
        {
            rightCard = transform.parent.GetChild(i).GetComponent<CardItem>();
            rightCard.GetComponent<RectTransform>().DOAnchorPosX(rightCard.CurrentPos.x, moveTime);
        }
        for (int i = index - 1; i >= 0; i--)
        {
            leftCard = transform.parent.GetChild(i).GetComponent<CardItem>();
            leftCard.GetComponent<RectTransform>().DOAnchorPosX(leftCard.CurrentPos.x, moveTime);
        }
        UIManager.Instance.ClearMoveClue(false);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (CantMove || BattleManager.Instance.MyBattleType != BattleManager.BattleType.Attack)
        {
            return;
        }
        isAttackCard = MyCardData.CardType == "攻擊";
    }

    private void Update()
    {
        if (isDrag)
        {
            OnCardPickup();
            if (Input.GetMouseButtonDown(0))
            {
                OnEndCardPickup();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                OnCancelCardPickup();
                ResetCardPos();
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (CantMove || BattleManager.Instance.MyBattleType != BattleManager.BattleType.Attack)
        {
            return;
        }
        if (!isDrag && eventData.button == PointerEventData.InputButton.Left)
        {
            isDrag = true;
            Cursor.visible = false;
            if (isAttackCard)
            {
                transform.DOScale(1.5f, moveTime);
                CardRectTransform.DOAnchorPosX(0, moveTime);
            }
            BattleManager.Instance.SwitchHandCardRaycast(false);
            BattleManager.Instance.InUseCardData = MyCardData;
        }
    }

    private void OnCardPickup()
    {
        if (!TryGetDragPosition(out Vector2 dragPosition))
        {
            return;
        }

        if (isAttackCard)
        {
            EventManager.Instance.DispatchEvent(EventDefinition.eventAttackLine, true, CardRectTransform.anchoredPosition, dragPosition);
        }
        else
        {
            CardRectTransform.anchoredPosition = dragPosition;
        }
    }
    private void OnCancelCardPickup()
    {
        isDrag = false;
        Cursor.visible = true;
        EventManager.Instance.DispatchEvent(EventDefinition.eventAttackLine, false, CardRectTransform.anchoredPosition, CardRectTransform.anchoredPosition);
        //CardRectTransform.anchoredPosition = CurrentPos;
        CardRectTransform.SetSiblingIndex(index);
        BattleManager.Instance.SwitchHandCardRaycast(true);
        BattleManager.Instance.InUseCardData = null;
    }
    private void OnEndCardPickup()
    {
        if (isAttackCard)
        {
            bool isEnemy = DetectEnemyByRay();
            if (isEnemy)
            {
                string location = BattleManager.Instance.GetEnemyKey(enemy.MyEnemyData);
                bool isInAttackRange = BattleManager.Instance.CheckEnemyInAttackRange(location, MyCardData.CardAttackDistance);
                if (isInAttackRange)
                {
                    if (GetUseCardCondition())
                    {
                        StartCoroutine(UseCard(location));
                    }

                }
                else
                {
                    BattleManager.Instance.ShowCharacterStatusClue(BattleManager.Instance.CurrentPlayer.StatusClueTrans, "不在攻擊範圍", 0);
                }
            }
        }
        else if (CardRectTransform.anchoredPosition.y >= 400 && GetUseCardCondition())
        {
            StartCoroutine(UseCard("Player"));
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        /* if (CantMove || BattleManager.Instance.MyBattleType != BattleManager.BattleType.Attack)
         {
             return;
         }
         isDrag = true;
         Cursor.visible = false;
         Vector2 dragPosition;
         RectTransform parentRect = transform.parent.GetComponent<RectTransform>();
         if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position, eventData.pressEventCamera, out dragPosition))
         {
             return;
         }
         if (isAttackCard)
         {
             EventManager.Instance.DispatchEvent(EventDefinition.eventAttackLine, true, CardRectTransform.anchoredPosition, dragPosition);
             //CheckRayToEnemy();
             return;
         }
         CardRectTransform.anchoredPosition = dragPosition;*/
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        /* if (CantMove || BattleManager.Instance.MyBattleType != BattleManager.BattleType.Attack)
         {
             return;
         }
         isDrag = false;
         Cursor.visible = true;
         if (isAttackCard)
         {
             EventManager.Instance.DispatchEvent(EventDefinition.eventAttackLine, false, CardRectTransform.anchoredPosition, CardRectTransform.anchoredPosition);
             CheckRayToEnemy();
             return;
         }
         if (CardRectTransform.anchoredPosition.y >= 400 && GetUseCardCondition())
         {
             UseCard("Player");
         }
         else
         {
             CardRectTransform.anchoredPosition = CurrentPos;
             CardRectTransform.SetSiblingIndex(index);
         }*/
    }

    private bool TryGetDragPosition(out Vector2 dragPosition)
    {
        RectTransform parentRect = transform.parent.GetComponent<RectTransform>();
        return RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, Input.mousePosition, Camera.main, out dragPosition);
    }

    private bool DetectEnemyByRay()
    {
        RectTransform attackLineHeadRect = ((UIAttackLine)UIManager.Instance.UIDict[nameof(UIAttackLine)]).HeadHotSpot;
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, attackLineHeadRect.position);
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 10f);
        if (Physics.Raycast(ray, out hit, 100) && hit.transform.TryGetComponent<Enemy>(out Enemy detectedEnemy))
        {
            enemy = detectedEnemy;
            return true;
        }
        return false;
    }

    private bool GetUseCardCondition()
    {
        PlayerData playerData = BattleManager.Instance.CurrentPlayerData;
        bool hasEnoughActionPoints = playerData.CurrentActionPoint >= MyCardData.CardCost;
        bool hasEnoughMana = playerData.Mana >= MyCardData.CardManaCost;
        bool isNotInAttackPhase = BattleManager.Instance.MyBattleType != BattleManager.BattleType.Attack;
        bool isCardCostNegative = MyCardData.CardCost < 0;
        if (!hasEnoughActionPoints)
        {
            BattleManager.Instance.ShowCharacterStatusClue(BattleManager.Instance.CurrentPlayer.StatusClueTrans, "行動力不足", 0);
        }
        else if (!hasEnoughMana)
        {
            BattleManager.Instance.ShowCharacterStatusClue(BattleManager.Instance.CurrentPlayer.StatusClueTrans, "魔力不足", 0);
        }
        return hasEnoughActionPoints && hasEnoughMana && !isNotInAttackPhase && !isCardCostNegative;
    }
    private IEnumerator UseCard(string target)
    {
        CardData cardData = MyCardData;
        CardRectTransform.DOScale(1f, 0);
        DataManager.Instance.HandCard.Remove(MyCardData);
        BattleManager.Instance.ConsumeActionPoint(cardData.CardCost);
        BattleManager.Instance.ConsumeMana(cardData.CardManaCost);
        string particlePath = cardData.CardSpecialEffect;
        if (particlePath != "")
        {
            Vector3 playerPos = BattleManager.Instance.PlayerTrans.position;
            Vector3 startPos = BattleManager.Instance.CurrentPlayer.AttackStartTrans.position;
            Vector3 destination = new Vector3(enemy.transform.position.x, enemy.transform.position.y, -1);
            yield return BattleManager.Instance.SetParticleEffect(BattleManager.Instance.PlayerAni, startPos, destination, particlePath, true);
        }
        int currentAttackCount = cardData.CardAttack + BattleManager.Instance.CurrentPlayer.AdditionPower;
        if (currentAttackCount != 0 && cardData.CardType != "詛咒")
        {
            BattleManager.Instance.TakeDamage(BattleManager.Instance.CurrentPlayerData, BattleManager.Instance.CurrentEnemyList[target], currentAttackCount, target, 0);
        }
        if (!cardData.CardRemove)
        {
            DataManager.Instance.UsedCardBag.Add(MyCardData);
        }
        else
        {
            DataManager.Instance.RemoveCardBag.Add(MyCardData);
        }
        if (BattleManager.Instance.CurrentNegativeState.ContainsKey(nameof(CharmEffect)))
        {
            BattleManager.Instance.ReduceNegativeState(nameof(CharmEffect));
            EventManager.Instance.DispatchEvent(EventDefinition.eventUseCard, this);
            EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
            gameObject.SetActive(false);
            yield break;
        }
        BattleManager.Instance.GetShield(BattleManager.Instance.CurrentPlayerData, cardData.CardShield);
        EventManager.Instance.DispatchEvent(EventDefinition.eventUseCard, this);
        yield return null;
        for (int i = 0; i < cardData.CardEffectList.Count; i++)
        {
            string effectID;
            int effectCount;
            effectID = cardData.CardEffectList[i].Item1;
            effectCount = cardData.CardEffectList[i].Item2;
            if (cardData.CardType == "能力")
            {
                BattleManager.Instance.AddState(BattleManager.Instance.CurrentAbilityList, effectID, effectCount);
                continue;
            }
            /*if (cardData.CardType == "陷阱")
            {
                BattleManager.Instance.CurrentTrapList.Add(BattleManager.Instance.CurrentPlayerLocation, effectID);
            }*/
            if (BattleManager.Instance.CurrentNegativeState.ContainsKey(nameof(CantIncreaseManaEffect)) && effectID == nameof(IncreaseManaEffect))
            {
                continue;
            }
            Transform statusClueTrans = BattleManager.Instance.CurrentPlayer.StatusClueTrans;
            string clueStrs = EffectFactory.Instance.CreateEffect(effectID).SetTitleText();
            float waitTime = 0.5f * i;
            EffectFactory.Instance.CreateEffect(effectID).ApplyEffect(effectCount, BattleManager.Instance.CurrentPlayerLocation, target);
            BattleManager.Instance.ShowCharacterStatusClue(statusClueTrans, clueStrs, waitTime);
        }

        if (cardData.CardRemove)
        {
            Material dissolveMaterial = BattleManager.Instance.DissolveEdgeMaterial;
            CardImage.material = dissolveMaterial;
            CardDescription.material = dissolveMaterial;
            CardName.material = dissolveMaterial;
            CardCost.material = dissolveMaterial;
            CardManaCost.material = dissolveMaterial;
            CardOutline.SetActive(false);
            isDrag = false;
            TweenCallback tweenCallback = () =>
            {
                gameObject.SetActive(false);
                OnCancelCardPickup();
            };
            BattleManager.Instance.SetDissolveMaterial(dissolveMaterial, 1.0f, 0, tweenCallback);
        }
        else
        {
            gameObject.SetActive(false);
            OnCancelCardPickup();
        }
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }
    public void RefreshCardOutline(params object[] args)
    {
        int currentActionPoint = BattleManager.Instance.CurrentPlayerData.CurrentActionPoint;
        if (currentActionPoint >= MyCardData.CardCost && BattleManager.Instance.CurrentPlayerData.Mana >= MyCardData.CardManaCost)
        {
            UIManager.Instance.ChangeOutline(this, true);
        }
        else
        {
            UIManager.Instance.ChangeOutline(this, false);
        }
    }
    private void EventReloadGame(params object[] args)
    {
        Destroy(gameObject);
    }

}
