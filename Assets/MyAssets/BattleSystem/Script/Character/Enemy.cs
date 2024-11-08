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
    private int actionRangeDistance;
    public int AdditionAttackCount { get; set; }
    public bool InRange { get; set; }
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
        string location = BattleManager.Instance.GetEnemyKey(MyEnemyData);
        float distance = BattleManager.Instance.GetRoute(location, BattleManager.Instance.CurrentLocationID, BattleManager.CheckEmptyType.EnemyAttack).Count;
        HandleAttack(location);
        InRange = BattleManager.Instance.EnemyAttackInRange(this, location);
        ResetUIElements();
        if (distance == 0)
        {
            HandleNoAttack();
        }
        else if (InRange)
        {
            HandleAttack(location);
        }
        else
        {
            HandleMove(location);
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

    private void HandleAttack(string location)
    {
        string attackOrder = MyEnemyData.AttackOrderStrs.ElementAt(MyEnemyData.CurrentAttackOrder).Item1;
        MyCheckEmptyType = BattleManager.CheckEmptyType.EnemyAttack;
        actionRangeDistance = MyEnemyData.AttackDistance;
        if (Enum.TryParse(attackOrder, out BattleManager.ActionRangeType attackType))
        {
            MyNextAttackActionRangeType = attackType;
            MyActionType = ActionType.Attack;
            ActiveAttack();
        }
        else if (attackOrder == "Shield")
        {
            ActivateShield();
        }
        else
        {
            ActivateEffect(attackOrder);
        }
        CurrentActionRangeTypeList = BattleManager.Instance.GetAcitonRangeTypeList(location, actionRangeDistance, MyCheckEmptyType, MyNextAttackActionRangeType);
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
        Image enemyEffectImage = enemyEffect.GetComponent<Image>();
        infoTitle.text = "效果";
        infoDescription.text = "施展未知效果。";
        enemyAttackIntentText.enabled = false;
        enemyEffectImage.sprite = EffectFactory.Instance.CreateEffect(attackOrder).SetIcon();
        enemyEffect.SetActive(true);
    }

    private void HandleMove(string location)
    {
        BattleManager.ActionRangeType actionRangeType = BattleManager.ActionRangeType.Default;
        actionRangeDistance = MyEnemyData.StepCount;
        infoTitle.text = "移動";
        infoDescription.text = "進行移動。";
        MyActionType = ActionType.Move;
        MyCheckEmptyType = BattleManager.CheckEmptyType.Move;
        CurrentActionRangeTypeList = BattleManager.Instance.GetAcitonRangeTypeList(location, actionRangeDistance, MyCheckEmptyType, actionRangeType);
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
        MySequence = DOTween.Sequence();
        string playerLocation = BattleManager.Instance.CurrentLocationID;
        string enemyLocation = BattleManager.Instance.GetEnemyKey(MyEnemyData);
        float distance = BattleManager.Instance.CalculateDistance(enemyLocation, playerLocation);
        RectTransform enemyRect = GetComponent<RectTransform>();
        int curveHeight = 500;
        Vector2 startPoint = enemyRect.localPosition;
        Vector2 endPoint = BattleManager.Instance.PlayerTrans.localPosition;
        Vector2 midPoint = new(startPoint.x + distance / 2, startPoint.y + curveHeight);
        Tween moveTween = DOTween.To((t) =>
        {
            Vector2 position = UIManager.Instance.GetBezierCurve(startPoint, midPoint, endPoint, t);
            enemyRect.anchoredPosition = position;
        }, 0, 1, 1).SetEase(Ease.InQuad);
        MySequence.Append(moveTween).AppendCallback(() => OnAttackComplete(true, enemyLocation, playerLocation)).Pause();
    }
    private void StraightChargeAttackSequence()
    {
        MySequence = DOTween.Sequence();
        string enemyLocation = BattleManager.Instance.GetEnemyKey(MyEnemyData);
        int attackDistance = MyEnemyData.AttackDistance;
        BattleManager.ActionRangeType actionRangeType = BattleManager.ActionRangeType.Linear;
        List<string> emptyPlaceList = BattleManager.Instance.GetAcitonRangeTypeList(enemyLocation, attackDistance, MyCheckEmptyType, actionRangeType);
        string destinationLocation = emptyPlaceList.Count > 0 ? emptyPlaceList[^1] : enemyLocation;
        RectTransform enemyRect = GetComponent<RectTransform>();
        int checkerboardPoint = BattleManager.Instance.GetCheckerboardPoint(destinationLocation);
        Vector2 destinationPos = BattleManager.Instance.CheckerboardTrans.GetChild(checkerboardPoint).GetComponent<RectTransform>().localPosition;
        Tween moveTween = enemyRect.DOAnchorPos(destinationPos, 0.25f);
        MySequence.Append(moveTween).AppendCallback(() => OnAttackComplete(true, enemyLocation, destinationLocation)).Pause();
    }
    private void OnAttackComplete(bool isKnockBack, string startLocation, string endLocation)
    {
        if (isKnockBack)
        {
            EffectFactory.Instance.CreateEffect("KnockBackEffect").ApplyEffect(1, startLocation, endLocation);
            BattleManager.Instance.ShowCharacterStatusClue(transform, EffectFactory.Instance.CreateEffect(endLocation).SetTitleText());
        }
        BattleManager.Instance.Replace(currentEnemyList, startLocation, endLocation);
        MySequence = null;
    }
    private void EventPlayerTurn(params object[] args)
    {
        BattleManager.Instance.RefreshCheckerboardList();
        RefreshAttackIntent();
    }
    private void EventRefreshUI(params object[] args)
    {
        enemyAttackIntentText.text = MyEnemyData.CurrentAttack.ToString();
    }
}