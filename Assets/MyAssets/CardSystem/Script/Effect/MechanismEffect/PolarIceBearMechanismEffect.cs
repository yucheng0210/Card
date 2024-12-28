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
        longDistanceEnemy.IsSuspendedAnimation = true;
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

            int healthCount = BattleManager.Instance.GetPercentage(longDistanceEnemyData.MaxHealth, 30);
            if (!longDistanceEnemy.IsSuspendedAnimation)
            {
                return;
            }
            if (longDistanceEnemyData.CurrentHealth <= 0 && meleeEnemyData.CurrentHealth > healthCount)
            {
                string effectName = "HealthBalanceEffect=" + healthCount.ToString();
                BattleManager.Instance.TemporaryChangeEffect(meleeEnemy, effectName);
                meleeEnemy.TargetLocation = longDistanceEnemyLocation;
                longDistanceEnemy.MyActionType = Enemy.ActionType.None;
                longDistanceEnemy.InfoTitle.text = "?";
                longDistanceEnemy.InfoDescription.text = "???";
                longDistanceEnemy.IsSuspendedAnimation = false;
            }
            else if (isInExchangeRange)
            {
                BattleManager.Instance.ExchangePos(meleeEnemyData.EnemyTrans, currentEnemyList, meleeEnemyLocation, longDistanceEnemyData.EnemyTrans, longDistanceEnemyLocation, currentEnemyList);
            }
        }
        if (args[5] == meleeEnemyData)
        {
            if (BattleManager.Instance.CurrentOnceBattlePositiveList.ContainsKey(nameof(ExplosiveMarkEffect)))
            {
                BattleManager.Instance.TakeDamage(meleeEnemyData, BattleManager.Instance.CurrentPlayerData, BattleManager.Instance.CurrentOnceBattlePositiveList[nameof(ExplosiveMarkEffect)], BattleManager.Instance.CurrentLocationID, 0);
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
