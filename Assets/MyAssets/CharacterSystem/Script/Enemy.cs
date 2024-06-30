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
    private Text enemyAttackIntentText;
    [Header("攻擊意圖")]
    [SerializeField]
    private GameObject enemyAttack;
    [SerializeField]
    private GameObject enemyShield;
    [SerializeField]
    private GameObject enemyMove;

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
    }
    private void Start()
    {
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
            string playerLocation = BattleManager.Instance.CurrentLocationID;
            BattleManager.CheckEmptyType type = BattleManager.CheckEmptyType.EnemyAttack;
            bool checkTerrainObstacles = BattleManager.Instance.CheckTerrainObstacles(location, enemyData.AlertDistance, playerLocation, type);
            enemyAttack.SetActive(false);
            enemyShield.SetActive(false);
            enemyMove.SetActive(false);
            enemyAttackIntentText.enabled = true;
            if (distance <= enemyData.AttackDistance && !checkTerrainObstacles)
            {
                switch (enemyData.AttackOrderStrs[enemyData.CurrentAttackOrder])
                {
                    case "Attack":
                        MyAttackType = AttackType.Attack;
                        enemyAttackIntentText.text = randomAttack.ToString();
                        enemyAttack.SetActive(true);
                        break;
                    case "Shield":
                        MyAttackType = AttackType.Shield;
                        enemyAttackIntentText.text = (randomAttack / 2).ToString();
                        enemyShield.SetActive(true);
                        break;
                    default:
                        MyAttackType = AttackType.Effect;
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
