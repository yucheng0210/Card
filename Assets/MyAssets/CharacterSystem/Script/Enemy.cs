using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor.Animations;

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
    public Image EnemyImage { get { return enemyImage; } set { enemyImage = value; } }
    public GameObject EnemyEffectImage { get { return enemyEffect; } set { enemyEffect = value; } }
    public Animator MyAnimator { get { return myAnimator; } set { myAnimator = value; } }
    public int EnemyID { get; set; }
    public string EnemyLocation { get; set; }
    public AttackType MyAttackType { get; set; }
    public bool IsDeath { get; set; }
    public Dictionary<string, int> EnemyOnceBattlePositiveList { get; set; }
    public enum AttackType
    {
        Move,
        Attack,
        Shield,
        Effect
    }
    private void Start()
    {
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
        EventManager.Instance.AddEventRegister(EventDefinition.eventMove, EventMove);
        EnemyData enemyData = BattleManager.Instance.CurrentEnemyList[EnemyLocation];
        enemyData.CurrentAttack = enemyData.MinAttack;
        EnemyOnceBattlePositiveList = new Dictionary<string, int>();
    }
    private void OnDisable()
    {
        EventManager.Instance.RemoveEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
        EventManager.Instance.RemoveEventRegister(EventDefinition.eventMove, EventMove);
    }
    private void RefrAttackIntent()
    {
        EnemyData enemyData = BattleManager.Instance.CurrentEnemyList[EnemyLocation];
        float distance = BattleManager.Instance.GetDistance(EnemyLocation);
        enemyAttack.SetActive(false);
        enemyShield.SetActive(false);
        enemyMove.SetActive(false);
        enemyEffect.SetActive(false);
        enemyAttackIntentText.enabled = true;
        if (distance <= enemyData.AttackDistance && BattleManager.Instance.CheckUnBlock(EnemyLocation, BattleManager.Instance.CurrentLocationID))
        {
            string attackOrder = enemyData.AttackOrderStrs.ElementAt(enemyData.CurrentAttackOrder).Item1;
            switch (attackOrder)
            {
                case "Attack":
                    MyAttackType = AttackType.Attack;
                    enemyAttackIntentText.text = enemyData.CurrentAttack.ToString();
                    enemyAttack.SetActive(true);
                    break;
                case "Shield":
                    MyAttackType = AttackType.Shield;
                    enemyAttackIntentText.text = (enemyData.CurrentAttack / 2).ToString();
                    enemyShield.SetActive(true);
                    break;
                default:
                    Image enemyEffectImage = enemyEffect.GetComponent<Image>();
                    MyAttackType = AttackType.Effect;
                    enemyAttackIntentText.enabled = false;
                    enemyEffectImage.sprite = EffectFactory.Instance.CreateEffect(attackOrder).SetIcon();
                    enemyEffect.SetActive(true);
                    break;
            }
        }
        else
        {
            MyAttackType = AttackType.Move;
            enemyAttackIntentText.enabled = false;
            enemyMove.SetActive(true);
        }
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }
    private void EventPlayerTurn(params object[] args)
    {
        BattleManager.Instance.RefreshCheckerboardList();
        RefrAttackIntent();
    }
    private void EventMove(params object[] args)
    {
        RefrAttackIntent();
    }
}
