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
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, meleeEnemyData, EventPlayerTurn);
        EventManager.Instance.AddEventRegister(EventDefinition.eventMove, meleeEnemyData, EventMove);
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, meleeEnemyData, EventTakeDamage);
    }
    private void EventPlayerTurn(params object[] args)
    {
        meleeEnemyLocation = currentEnemyList.ElementAt(0).Key;
        longDistanceEnemyLocation = currentEnemyList.ElementAt(1).Key;
    }
    private void EventMove(params object[] args)
    {
        List<string> surroundingsLocationList = BattleManager.Instance.GetActionRangeTypeList(longDistanceEnemyLocation, 1,
        BattleManager.CheckEmptyType.EnemyAttack, BattleManager.ActionRangeType.Surrounding);
        isInExchangeRange = surroundingsLocationList.Contains(BattleManager.Instance.CurrentLocationID);
    }
    private void EventTakeDamage(params object[] args)
    {
        if (args[5] == longDistanceEnemyData)
        {
            if (isInExchangeRange)
            {
                BattleManager.Instance.ExchangePos(meleeEnemyData.EnemyTrans, currentEnemyList, meleeEnemyLocation, longDistanceEnemyData.EnemyTrans, longDistanceEnemyLocation, currentEnemyList);
            }
            int healthCount = BattleManager.Instance.GetPercentage(longDistanceEnemyData.MaxHealth, 30);
            if (longDistanceEnemyData.CurrentHealth <= BattleManager.Instance.GetPercentage(longDistanceEnemyData.MaxHealth, 40) && meleeEnemyData.CurrentHealth > healthCount)
            {
                string effectName = "HealthBalanceEffect=" + healthCount.ToString();
                BattleManager.Instance.TemporaryChangeEffect(meleeEnemy, effectName);
                meleeEnemy.TargetLocation = longDistanceEnemyLocation;
            }
        }
    }
    public string SetTitleText()
    {
        return "";
    }

    public string SetDescriptionText()
    {
        return "";
    }
}
