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
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        meleeEnemyLocation = BattleManager.Instance.CurrentEnemyList.ElementAt(0).Key;
        longDistanceEnemyLocation = BattleManager.Instance.CurrentMinionsList.ElementAt(0).Key;
        meleeEnemyData = BattleManager.Instance.CurrentEnemyList[meleeEnemyLocation];
        longDistanceEnemyData = BattleManager.Instance.CurrentMinionsList[longDistanceEnemyLocation];
        meleeEnemy = meleeEnemyData.EnemyTrans.GetComponent<Enemy>();
        longDistanceEnemy = longDistanceEnemyData.EnemyTrans.GetComponent<Enemy>();
        EventManager.Instance.AddEventRegister(EventDefinition.eventMove, meleeEnemyData, EventMove);
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, meleeEnemyData, EventTakeDamage);
    }
    private void EventMove(params object[] args)
    {
        List<string> surroundingsLocationList = BattleManager.Instance.GetActionRangeTypeList(longDistanceEnemyLocation, 1, BattleManager.CheckEmptyType.EnemyAttack, BattleManager.ActionRangeType.Surrounding);
        isInExchangeRange = surroundingsLocationList.Contains(BattleManager.Instance.CurrentLocationID);
    }
    private void EventTakeDamage(params object[] args)
    {
        if (args[5] == longDistanceEnemyData && isInExchangeRange)
        {
            BattleManager.Instance.ExchangePos(meleeEnemyData.EnemyTrans, BattleManager.Instance.CurrentEnemyList, meleeEnemyLocation, longDistanceEnemyData.EnemyTrans, longDistanceEnemyLocation, BattleManager.Instance.CurrentMinionsList);
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
