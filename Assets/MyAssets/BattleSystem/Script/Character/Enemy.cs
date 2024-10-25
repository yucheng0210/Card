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
    //public string EnemyLocation { get; set; }
    public ActionType MyActionType { get; set; }
    public BattleManager.ActionRangeType MyActionRangeType { get; set; }
    public BattleManager.CheckEmptyType MyCheckEmptyType { get; set; }
    public bool IsDeath { get; set; }
    public Dictionary<string, int> EnemyOnceBattlePositiveList { get; set; }
    private Dictionary<string, EnemyData> currentEnemyList = new();
    public EnemyData MyEnemyData { get; set; }
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
        string location = BattleManager.Instance.GetEnemyKey(MyEnemyData, BattleManager.Instance.CurrentEnemyList);
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
        EventManager.Instance.AddEventRegister(EventDefinition.eventMove, EventMove);
        EnemyOnceBattlePositiveList = new Dictionary<string, int>();
        currentEnemyList = BattleManager.Instance.CurrentEnemyList;
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
        EventManager.Instance.RemoveEventRegister(EventDefinition.eventMove, EventMove);
    }
    private void RefreshAttackIntent()
    {
        string location = BattleManager.Instance.GetEnemyKey(MyEnemyData, currentEnemyList);
        float distance = BattleManager.Instance.GetRoute(location, BattleManager.Instance.CurrentLocationID, BattleManager.CheckEmptyType.EnemyAttack).Count;
        ResetUIElements();
        if (distance == 0)
        {
            HandleNoAttack();
        }
        else if (distance <= MyEnemyData.AttackDistance)
        {
            HandleAttack();
        }
        else
        {
            HandleMove();
        }
        CurrentActionRangeTypeList = BattleManager.Instance.GetAcitonRangeTypeList(location, actionRangeDistance, MyCheckEmptyType, MyActionRangeType);
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
        string attackOrder = MyEnemyData.AttackOrderStrs.ElementAt(MyEnemyData.CurrentAttackOrder).Item1;
        Dictionary<string, BattleManager.ActionRangeType> attackTypeMap = new Dictionary<string, BattleManager.ActionRangeType>
        {
            { "LinearAttack", BattleManager.ActionRangeType.Linear },
            { "SurroundingAttack", BattleManager.ActionRangeType.Surrounding },
            { "ConeAttack",BattleManager.ActionRangeType.Cone }
        };
        actionRangeDistance = MyEnemyData.AttackDistance;
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
        int shieldCount = MyEnemyData.AttackOrderStrs.ElementAt(MyEnemyData.CurrentAttackOrder).Item2;
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
        actionRangeDistance = MyEnemyData.AttackDistance;
        MyActionType = ActionType.Effect;
        MyActionRangeType = EffectFactory.Instance.CreateEffect(attackOrder).SetEffectAttackType();
        MyCheckEmptyType = BattleManager.CheckEmptyType.EnemyAttack;
        Image enemyEffectImage = enemyEffect.GetComponent<Image>();
        infoTitle.text = "效果";
        infoDescription.text = "施展未知效果。";
        enemyAttackIntentText.enabled = false;
        enemyEffectImage.sprite = EffectFactory.Instance.CreateEffect(attackOrder).SetIcon();
        enemyEffect.SetActive(true);
    }

    private void HandleMove()
    {
        actionRangeDistance = MyEnemyData.StepCount;
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
        enemyAttackIntentText.text = MyEnemyData.CurrentAttack.ToString();
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
