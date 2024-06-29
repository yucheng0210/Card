using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private Image alertImage;
    [SerializeField]
    private Slider enemyHealthSlider;

    [SerializeField]
    private Text enemyHealthText;

    [SerializeField]
    private Text enemyAttackIntentText;
    [Header("攻擊意圖")]
    [SerializeField]
    private GameObject enemyAttack;
    [SerializeField]
    private GameObject enemyShield;

    public Image EnemyAlert
    {
        get { return alertImage; }
        set { alertImage = value; }
    }
    public Image EnemyImage { get; set; }
    public int EnemyID { get; set; }
    public string EnemyLocation { get; set; }
    public AttackType MyAttackType { get; set; }
    public enum AttackType
    {
        Move,
        Attack,
        Shield,
        Effect
    }

    private void Awake()
    {
        EnemyImage = GetComponent<Image>();
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
        EventManager.Instance.AddEventRegister(EventDefinition.eventMove, EventMove);
    }
    private void OnDisable()
    {
        EventManager.Instance.RemoveEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
    }
    private void RefrAttackIntent()
    {
        int randomAttack = UnityEngine.Random.Range(DataManager.Instance.EnemyList[EnemyID].MinAttack, DataManager.Instance.EnemyList[EnemyID].MaxAttack + 1);
        EnemyData enemyData = BattleManager.Instance.CurrentEnemyList[EnemyLocation];
        enemyData.CurrentAttack = randomAttack;
        for (int i = 0; i < BattleManager.Instance.CurrentEnemyList.Count; i++)
        {
            string location = BattleManager.Instance.CurrentEnemyList.ElementAt(i).Key;
            float distance = BattleManager.Instance.GetDistance(location);
            RectTransform enemyTrans = enemyData.EnemyTrans;
            bool checkTerrainObstacles = BattleManager.Instance.CheckTerrainObstacles(location, enemyData.AlertDistance, BattleManager.Instance.CurrentLocationID, BattleManager.CheckEmptyType.EnemyAttack);
            if (distance <= enemyData.AttackDistance && !checkTerrainObstacles)
            {
                switch (enemyData.AttackOrderStrs[enemyData.CurrentAttackOrder])
                {
                    case "Attack":
                        MyAttackType = AttackType.Attack;
                        enemyAttackIntentText.text = randomAttack.ToString();
                        break;
                    case "Shield":
                        MyAttackType = AttackType.Shield;
                        enemyAttackIntentText.text = (randomAttack / 2).ToString();
                        break;
                    default:
                        MyAttackType = AttackType.Effect;
                        break;
                }
            }
            else
                MyAttackType = AttackType.Move;
            EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
        }
    }
    private void EventPlayerTurn(params object[] args)
    {
        RefrAttackIntent();
    }
    private void EventMove(params object[] args)
    {
        RefrAttackIntent();
    }
}
