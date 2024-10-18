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
    public string EnemyLocation { get; set; }
    public ActionType MyActionType { get; set; }
    public BattleManager.ActionRangeType MyActionRangeType { get; set; }
    public BattleManager.CheckEmptyType MyCheckEmptyType { get; set; }
    public bool IsDeath { get; set; }
    public Dictionary<string, int> EnemyOnceBattlePositiveList { get; set; }
    private Dictionary<string, EnemyData> currentEnemyList = new();
    private EnemyData enemyData = new EnemyData();
    private int actionRangeDistance;
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
        EnemyOnceBattlePositiveList = new Dictionary<string, int>();
        currentEnemyList = BattleManager.Instance.CurrentEnemyList;
        enemyData = currentEnemyList.ContainsKey(EnemyLocation) ? currentEnemyList[EnemyLocation] : BattleManager.Instance.CurrentMinionsList[EnemyLocation];
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
        EventManager.Instance.RemoveEventRegister(EventDefinition.eventMove, EventMove);
    }
    private void RefreshAttackIntent()
    {
        float distance = BattleManager.Instance.GetRoute(EnemyLocation, BattleManager.Instance.CurrentLocationID, BattleManager.CheckEmptyType.EnemyAttack).Count;
        ResetUIElements();
        if (distance == 0)
        {
            HandleNoAttack();
        }
        else if (distance <= enemyData.AttackDistance)
        {
            HandleAttack();
        }
        else
        {
            HandleMove();
        }
        CurrentActionRangeTypeList = BattleManager.Instance.GetAcitonRangeTypeList(EnemyLocation, actionRangeDistance, MyCheckEmptyType, MyActionRangeType);
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

    private void HandleAttack()
    {
        string attackOrder = enemyData.AttackOrderStrs.ElementAt(enemyData.CurrentAttackOrder).Item1;
        Dictionary<string, BattleManager.ActionRangeType> attackTypeMap = new Dictionary<string, BattleManager.ActionRangeType>
        {
            { "LinearAttack", BattleManager.ActionRangeType.Linear },
            { "SurroundingAttack", BattleManager.ActionRangeType.Surrounding },
            { "ConeAttack",BattleManager.ActionRangeType.Cone }
        };
        actionRangeDistance = enemyData.AttackDistance;
        if (attackTypeMap.TryGetValue(attackOrder, out BattleManager.ActionRangeType attackType))
        {
            MyActionRangeType = attackType;
            MyActionType = ActionType.Attack;
            MyCheckEmptyType = BattleManager.CheckEmptyType.EnemyAttack;
            Attack();
        }
        else if (attackOrder == "Shield")
        {
            ActivateShield();
        }
        else
        {
            ActivateEffect(attackOrder);
        }
    }

    private void ActivateShield()
    {
        int shieldCount = enemyData.AttackOrderStrs.ElementAt(enemyData.CurrentAttackOrder).Item2;
        infoTitle.text = "護盾";
        infoDescription.text = "產生護盾。";
        MyActionType = ActionType.Shield;
        MyActionRangeType = BattleManager.ActionRangeType.None;
        MyCheckEmptyType = BattleManager.CheckEmptyType.EnemyAttack;
        enemyAttackIntentText.text = shieldCount.ToString();
        enemyShield.SetActive(true);
    }

    private void ActivateEffect(string attackOrder)
    {
        actionRangeDistance = enemyData.AttackDistance;
        MyActionType = ActionType.Effect;
        MyActionRangeType = EffectFactory.Instance.CreateEffect(attackOrder).SetEffectAttackType();
        MyCheckEmptyType = BattleManager.CheckEmptyType.EnemyAttack;
        Image enemyEffectImage = enemyEffect.GetComponent<Image>();
        infoTitle.text = EffectFactory.Instance.CreateEffect(attackOrder).SetTitleText();
        infoDescription.text = EffectFactory.Instance.CreateEffect(attackOrder).SetDescriptionText();
        enemyAttackIntentText.enabled = false;
        enemyEffectImage.sprite = EffectFactory.Instance.CreateEffect(attackOrder).SetIcon();
        enemyEffect.SetActive(true);
    }

    private void HandleMove()
    {
        actionRangeDistance = enemyData.StepCount;
        infoTitle.text = "移動";
        infoDescription.text = "進行移動。";
        MyActionType = ActionType.Move;
        MyActionRangeType = BattleManager.ActionRangeType.Default;
        MyCheckEmptyType = BattleManager.CheckEmptyType.Move;
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

    private void Attack()
    {
        infoTitle.text = "攻擊";
        infoDescription.text = "發動攻擊。";
        enemyAttackIntentText.text = enemyData.CurrentAttack.ToString();
        enemyAttack.SetActive(true);
    }
    private void EventPlayerTurn(params object[] args)
    {
        BattleManager.Instance.RefreshCheckerboardList();
        RefreshAttackIntent();
    }
    private void EventMove(params object[] args)
    {
        //CurrentActionRangeTypeList = BattleManager.Instance.GetAcitonRangeTypeList(EnemyLocation, actionRangeDistance, MyCheckEmptyType, MyActionRangeType);
        // RefreshAttackIntent();
    }
}
