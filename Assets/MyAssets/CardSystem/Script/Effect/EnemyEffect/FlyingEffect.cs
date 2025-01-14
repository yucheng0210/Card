using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEffect : IEffect
{
    private EnemyData enemyData;
    private int reciprocalCount = 3;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        enemyData = BattleManager.Instance.CurrentEnemyList[fromLocation];
        enemyData.DodgeChance = value;
        EventManager.Instance.AddEventRegister(EventDefinition.eventEnemyTurn, enemyData, EventEnemyTurn);
    }
    private void EventEnemyTurn(params object[] args)
    {
        if (reciprocalCount == 0)
        {
            enemyData.DodgeChance = 0;
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventEnemyTurn, EventEnemyTurn);
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
