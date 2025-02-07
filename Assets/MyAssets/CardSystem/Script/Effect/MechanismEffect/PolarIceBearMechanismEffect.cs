using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PolarIceBearMechanismEffect : IEffect
{
    private EnemyData meleeEnemyData;
    private EnemyData longDistanceEnemyData;
    private Enemy meleeEnemy;
    private Enemy longDistanceEnemy;
    private string meleeEnemyLocation;
    private string longDistanceEnemyLocation;
    private bool isInExchangeRange;
    private int exchangeCount = 1;
    private Dictionary<string, EnemyData> currentEnemyList;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        currentEnemyList = BattleManager.Instance.CurrentEnemyList;
        meleeEnemyLocation = currentEnemyList.ElementAt(0).Key;
        longDistanceEnemyLocation = currentEnemyList.ElementAt(1).Key;
        meleeEnemyData = currentEnemyList[meleeEnemyLocation];
        longDistanceEnemyData = currentEnemyList[longDistanceEnemyLocation];
        meleeEnemy = meleeEnemyData.EnemyTrans.GetComponent<Enemy>();
        longDistanceEnemy = longDistanceEnemyData.EnemyTrans.GetComponent<Enemy>();
        longDistanceEnemy.IsSuspendedAnimation = true;
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, meleeEnemyData, EventPlayerTurn);
        EventManager.Instance.AddEventRegister(EventDefinition.eventMove, meleeEnemyData, EventMove);
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, meleeEnemyData, EventTakeDamage);
    }
    private void EventPlayerTurn(params object[] args)
    {
        meleeEnemyLocation = BattleManager.Instance.GetEnemyKey(meleeEnemyData);
        longDistanceEnemyLocation = BattleManager.Instance.GetEnemyKey(longDistanceEnemyData);
        exchangeCount = 1;
    }
    private void EventMove(params object[] args)
    {
        List<string> surroundingsLocationList = BattleManager.Instance.GetActionRangeTypeList(longDistanceEnemyLocation, 1,
        BattleManager.CheckEmptyType.EnemyAttack, BattleManager.ActionRangeType.Surrounding);
        isInExchangeRange = surroundingsLocationList.Contains(BattleManager.Instance.CurrentPlayerLocation);
    }
    private void EventTakeDamage(params object[] args)
    {
        if (meleeEnemyData.CurrentHealth <= 0)
        {
            longDistanceEnemy.IsSuspendedAnimation = false;
        }
        if (args[5] == longDistanceEnemyData)
        {
            int healthCount = Mathf.Min(BattleManager.Instance.GetPercentage(longDistanceEnemyData.MaxHealth, 30), meleeEnemyData.CurrentHealth - 1);
            if (!longDistanceEnemy.IsSuspendedAnimation)
            {
                return;
            }
            if (longDistanceEnemyData.CurrentHealth <= 0)
            {
                string effectName = "HealthBalanceEffect=" + healthCount.ToString();
                BattleManager.Instance.TemporaryChangeEffect(meleeEnemy, effectName, longDistanceEnemyLocation);
                longDistanceEnemy.MyActionType = Enemy.ActionType.None;
                longDistanceEnemy.InfoTitle.text = "?";
                longDistanceEnemy.InfoDescription.text = "???";
                longDistanceEnemy.IsSuspendedAnimation = false;
            }
            else if (isInExchangeRange && exchangeCount > 0)
            {
                Transform meleeEnemyTrans = meleeEnemyData.EnemyTrans;
                Transform longEnemyTrans = longDistanceEnemyData.EnemyTrans;
                BattleManager.Instance.ExchangePos(meleeEnemyTrans, currentEnemyList, meleeEnemyLocation, longEnemyTrans, longDistanceEnemyLocation, currentEnemyList);
                meleeEnemy.RefreshAttackIntent();
                longDistanceEnemy.RefreshAttackIntent();
                exchangeCount--;
            }
        }
    }
    public string SetTitleText()
    {
        return "極冬領域";
    }

    public string SetDescriptionText()
    {
        return "";
    }
}
