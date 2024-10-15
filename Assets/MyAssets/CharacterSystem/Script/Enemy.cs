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
    public int EnemyID { get; set; }
    public string EnemyLocation { get; set; }
    public AttackType MyAttackType { get; set; }
    public bool IsDeath { get; set; }
    public Dictionary<string, int> EnemyOnceBattlePositiveList { get; set; }
    private Dictionary<string, EnemyData> currentEnemyList = new();
    private EnemyData enemyData = new EnemyData();
    public enum AttackType
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
        Debug.Log(distance);
        enemyAttack.SetActive(false);
        enemyShield.SetActive(false);
        enemyMove.SetActive(false);
        enemyEffect.SetActive(false);
        enemyAttackIntentText.enabled = true;
        if (distance == 0)
        {
            MyAttackType = AttackType.None;
            enemyAttackIntentText.text = "?";
        }
        else if (distance <= enemyData.AttackDistance)
        {
            string attackOrder = enemyData.AttackOrderStrs.ElementAt(enemyData.CurrentAttackOrder).Item1;
            switch (attackOrder)
            {
                case "Attack":
                    infoTitle.text = "攻擊";
                    infoDescription.text = "發動攻擊。";
                    MyAttackType = AttackType.Attack;
                    enemyAttackIntentText.text = enemyData.CurrentAttack.ToString();
                    enemyAttack.SetActive(true);
                    break;
                case "Shield":
                    infoTitle.text = "護盾";
                    infoDescription.text = "產生護盾。";
                    MyAttackType = AttackType.Shield;
                    enemyAttackIntentText.text = (enemyData.CurrentAttack / 2).ToString();
                    enemyShield.SetActive(true);
                    break;
                default:
                    Image enemyEffectImage = enemyEffect.GetComponent<Image>();
                    infoTitle.text = EffectFactory.Instance.CreateEffect(attackOrder).SetTitleText();
                    infoDescription.text = EffectFactory.Instance.CreateEffect(attackOrder).SetDescriptionText();
                    MyAttackType = AttackType.Effect;
                    enemyAttackIntentText.enabled = false;
                    enemyEffectImage.sprite = EffectFactory.Instance.CreateEffect(attackOrder).SetIcon();
                    enemyEffect.SetActive(true);
                    break;
            }
        }
        else
        {
            infoTitle.text = "移動";
            infoDescription.text = "進行移動。";
            MyAttackType = AttackType.Move;
            enemyAttackIntentText.enabled = false;
            enemyMove.SetActive(true);
        }
        EventTrigger eventTrigger = infoGroupTrans.GetComponent<EventTrigger>();
        UnityAction unityAction_1 = () => { infoGroupTrans.GetChild(0).gameObject.SetActive(true); };
        UnityAction unityAction_2 = () => { infoGroupTrans.GetChild(0).gameObject.SetActive(false); };
        BattleManager.Instance.SetEventTrigger(eventTrigger, unityAction_1, unityAction_2);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }
    private void EventPlayerTurn(params object[] args)
    {
        BattleManager.Instance.RefreshCheckerboardList();
        RefreshAttackIntent();
    }
    private void EventMove(params object[] args)
    {
        RefreshAttackIntent();
    }
}
