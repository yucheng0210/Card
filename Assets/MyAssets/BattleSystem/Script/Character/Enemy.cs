using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private Text enemyAttackIntentText;
    [SerializeField]
    private Image enemyImage;
    [SerializeField]
    private Animator myAnimator;
    [Header("攻擊意圖")]
    [SerializeField]
    private GameObject enemyEffect;
    [SerializeField]
    private GameObject enemyAttack;
    [SerializeField]
    private GameObject enemyShield;
    [SerializeField]
    private GameObject enemyMove;
    [SerializeField]
    private Transform infoGroupTrans;
    [SerializeField]
    private Text infoTitle;
    [SerializeField]
    private Text infoDescription;
    public Image EnemyImage { get { return enemyImage; } set { enemyImage = value; } }
    public GameObject EnemyEffectImage { get { return enemyEffect; } set { enemyEffect = value; } }
    public Animator MyAnimator { get { return myAnimator; } set { myAnimator = value; } }
    public List<string> CurrentActionRangeTypeList { get; set; }
    public int EnemyID { get; set; }
    public ActionType MyActionType { get; set; }
    public BattleManager.ActionRangeType MyNextAttackActionRangeType { get; set; }
    public BattleManager.CheckEmptyType MyCheckEmptyType { get; set; }
    public bool IsDeath { get; set; }
    public Dictionary<string, int> EnemyOnceBattlePositiveList { get; set; }
    public Sequence MySequence { get; set; }
    private Dictionary<string, EnemyData> currentEnemyList = new();
    public EnemyData MyEnemyData { get; set; }
    public int CurrentAttackDistance { get; set; }
    public int AdditionAttackCount { get; set; }
    public bool InRange { get; set; }
    private string location;
    public enum ActionType
    {
        Move,
        Attack,
        Shield,
        Effect,
        None,
    }
    private void Start()
    {
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
        EventManager.Instance.AddEventRegister(EventDefinition.eventMove, EventMove);
        EventManager.Instance.AddEventRegister(EventDefinition.eventRefreshUI, EventRefreshUI);
        EnemyOnceBattlePositiveList = new Dictionary<string, int>();
        currentEnemyList = BattleManager.Instance.CurrentEnemyList;
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
    }
    private void RefreshAttackIntent()
    {
        MySequence = null;
        location = BattleManager.Instance.GetEnemyKey(MyEnemyData);
        float distance = BattleManager.Instance.GetRoute(location, BattleManager.Instance.CurrentLocationID, BattleManager.CheckEmptyType.EnemyAttack).Count;
        HandleAttack(true);
        ResetUIElements();
        BattleManager.Instance.CheckPlayerLocationInRange(this);
        if (distance == 0)
        {
            HandleNoAttack();
        }
        else if (InRange)
        {
            HandleAttack(false);
        }
        else
        {
            HandleMove();
        }
        SetInfoGroupEventTrigger();
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }
    private void ResetUIElements()
    {
        enemyAttack.SetActive(false);
        enemyShield.SetActive(false);
        enemyMove.SetActive(false);
        enemyEffect.SetActive(false);
        enemyAttackIntentText.enabled = true;
    }

    private void HandleNoAttack()
    {
        MyActionType = ActionType.None;
        enemyAttackIntentText.text = "?";
    }

    private void HandleAttack(bool isCheck)
    {
        string attackOrder = MyEnemyData.AttackOrderStrs.ElementAt(MyEnemyData.CurrentAttackOrder).Item1;
        MyCheckEmptyType = BattleManager.CheckEmptyType.EnemyAttack;
        CurrentAttackDistance = MyEnemyData.AttackDistance;
        if (Enum.TryParse(attackOrder, out BattleManager.ActionRangeType attackType))
        {
            MyNextAttackActionRangeType = attackType;
            MyActionType = ActionType.Attack;
            if (!isCheck)
            {
                ActiveAttack();
            }
        }
        else if (attackOrder == "Shield")
        {
            ActivateShield();
        }
        else
        {
            ActivateEffect(attackOrder);
        }
        CurrentActionRangeTypeList = BattleManager.Instance.GetActionRangeTypeList(location, CurrentAttackDistance, MyCheckEmptyType, MyNextAttackActionRangeType);
    }

    private void ActivateShield()
    {
        int shieldCount = MyEnemyData.AttackOrderStrs.ElementAt(MyEnemyData.CurrentAttackOrder).Item2;
        infoTitle.text = "護盾";
        infoDescription.text = "產生護盾。";
        MyActionType = ActionType.Shield;
        MyNextAttackActionRangeType = BattleManager.ActionRangeType.None;
        enemyAttackIntentText.text = shieldCount.ToString();
        enemyShield.SetActive(true);
    }

    private void ActivateEffect(string attackOrder)
    {
        MyActionType = ActionType.Effect;
        MyNextAttackActionRangeType = EffectFactory.Instance.CreateEffect(attackOrder).SetEffectAttackType();
        CurrentAttackDistance = EffectFactory.Instance.CreateEffect(attackOrder).SetEffectDistance();
        if (CurrentAttackDistance <= 0)
        {
            CurrentAttackDistance = MyEnemyData.AttackDistance;
        }
        Image enemyEffectImage = enemyEffect.GetComponent<Image>();
        infoTitle.text = "效果";
        infoDescription.text = "施展未知效果。";
        enemyAttackIntentText.enabled = false;
        enemyEffectImage.sprite = EffectFactory.Instance.CreateEffect(attackOrder).SetIcon();
        enemyEffect.SetActive(true);
    }

    private void HandleMove()
    {
        BattleManager.ActionRangeType actionRangeType = BattleManager.ActionRangeType.Default;
        CurrentAttackDistance = MyEnemyData.StepCount;
        infoTitle.text = "移動";
        infoDescription.text = "進行移動。";
        MyActionType = ActionType.Move;
        MyCheckEmptyType = BattleManager.CheckEmptyType.Move;
        CurrentActionRangeTypeList = BattleManager.Instance.GetActionRangeTypeList(location, CurrentAttackDistance, MyCheckEmptyType, actionRangeType);
        enemyAttackIntentText.enabled = false;
        enemyMove.SetActive(true);
    }

    private void SetInfoGroupEventTrigger()
    {
        EventTrigger eventTrigger = infoGroupTrans.GetComponent<EventTrigger>();
        UnityAction unityAction_1 = () => { infoGroupTrans.GetChild(0).gameObject.SetActive(true); };
        UnityAction unityAction_2 = () => { infoGroupTrans.GetChild(0).gameObject.SetActive(false); };
        BattleManager.Instance.SetEventTrigger(eventTrigger, unityAction_1, unityAction_2);
    }

    private void ActiveAttack()
    {
        infoTitle.text = "攻擊";
        infoDescription.text = "發動攻擊。";
        enemyAttackIntentText.text = MyEnemyData.CurrentAttack.ToString();
        enemyAttack.SetActive(true);
        SetAttackActionRangeType();
    }
    private void SetAttackActionRangeType()
    {
        switch (MyNextAttackActionRangeType)
        {
            case BattleManager.ActionRangeType.Jump:
                JumpAttackSequence();
                break;
            case BattleManager.ActionRangeType.StraightCharge:
                StraightChargeAttackSequence();
                break;
        }
    }
    private void JumpAttackSequence()
    {
        MySequence = DOTween.Sequence().Pause();
        string destinationLocation = BattleManager.Instance.CurrentLocationID;
        string enemyLocation = BattleManager.Instance.GetEnemyKey(MyEnemyData);
        float distance = BattleManager.Instance.CalculateDistance(enemyLocation, destinationLocation);
        int checkerboardPoint = BattleManager.Instance.GetCheckerboardPoint(destinationLocation);
        RectTransform destinationPlace = BattleManager.Instance.CheckerboardTrans.GetChild(checkerboardPoint).GetComponent<RectTransform>();
        RectTransform enemyRect = GetComponent<RectTransform>();
        int curveHeight = 350;
        Vector2 startPoint = enemyRect.localPosition;
        Vector2 endPoint = destinationPlace.localPosition;
        Vector2 midPoint = new(startPoint.x + distance / 2, startPoint.y + curveHeight);
        Tween moveTween = DOTween.To((t) =>
        {
            Vector2 position = UIManager.Instance.GetBezierCurve(startPoint, midPoint, endPoint, t);
            enemyRect.anchoredPosition = position;
        }, 0, 1, 1).SetEase(Ease.InQuad);
        MySequence.Append(moveTween).AppendCallback(() => OnAttackComplete(true, enemyLocation, destinationLocation));
    }
    private void StraightChargeAttackSequence()
    {
        MySequence = DOTween.Sequence().Pause();
        string enemyLocation = BattleManager.Instance.GetEnemyKey(MyEnemyData);
        int attackDistance = MyEnemyData.AttackDistance;
        BattleManager.ActionRangeType actionRangeType = BattleManager.ActionRangeType.Linear;
        List<string> emptyPlaceList = BattleManager.Instance.GetActionRangeTypeList(enemyLocation, attackDistance, MyCheckEmptyType, actionRangeType);
        string destinationLocation = emptyPlaceList.Count > 0 ? emptyPlaceList[^1] : enemyLocation;
        RectTransform enemyRect = GetComponent<RectTransform>();
        int checkerboardPoint = BattleManager.Instance.GetCheckerboardPoint(destinationLocation);
        Vector2 destinationPos = BattleManager.Instance.CheckerboardTrans.GetChild(checkerboardPoint).GetComponent<RectTransform>().localPosition;
        Tween moveTween = enemyRect.DOAnchorPos(destinationPos, 0.25f);
        MySequence.Append(moveTween).AppendCallback(() => OnAttackComplete(true, enemyLocation, destinationLocation));
    }
    private void OnAttackComplete(bool isKnockBack, string startLocation, string endLocation)
    {
        PlayerData playerData = BattleManager.Instance.CurrentPlayerData;
        if (isKnockBack)
        {
            EffectFactory.Instance.CreateEffect("KnockBackEffect").ApplyEffect(1, startLocation, endLocation);
        }
        if (InRange)
        {
            BattleManager.Instance.TakeDamage(MyEnemyData, playerData, MyEnemyData.CurrentAttack, BattleManager.Instance.CurrentLocationID, 0);
        }
        BattleManager.Instance.Replace(currentEnemyList, startLocation, endLocation);
        //Debug.Log(startLocation + "   " + endLocation);
    }
    private void EventPlayerTurn(params object[] args)
    {
        BattleManager.Instance.RefreshCheckerboardList();
        RefreshAttackIntent();
    }
    private void EventMove(params object[] args)
    {
        BattleManager.Instance.CheckPlayerLocationInRange(this);
        /* if (MyActionType == ActionType.Attack)
         {
             SetAttackActionRangeType();
             string location = BattleManager.Instance.GetEnemyKey(MyEnemyData);
             CurrentActionRangeTypeList = BattleManager.Instance.GetActionRangeTypeList(location, CurrentAttackDistance, MyCheckEmptyType, MyNextAttackActionRangeType);
         }*/
    }
    private void EventRefreshUI(params object[] args)
    {
        enemyAttackIntentText.text = MyEnemyData.CurrentAttack.ToString();
    }
}