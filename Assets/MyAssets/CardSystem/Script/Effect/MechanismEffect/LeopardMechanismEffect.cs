using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeopardMechanismEffect : IEffect
{
    private EnemyData enemyData;
    private Enemy enemy;
    private int count;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        enemyData = BattleManager.Instance.CurrentEnemyList[fromLocation];
        enemy = enemyData.EnemyTrans.GetComponent<Enemy>();
        EventManager.Instance.AddEventRegister(EventDefinition.eventEnemyTurn, EventEnemyTurn);
    }
    private void EventEnemyTurn(params object[] args)
    {
        if (!enemy.InRange && enemy.MyActionType == Enemy.ActionType.Attack)
        {
            count++;
            EventManager.Instance.AddEventRegister(EventDefinition.eventAttack, EventAttack);
        }
        else
        {
            count = 0;
        }
    }
    private void EventAttack(params object[] args)
    {
        int limitCount = Mathf.Min(count, DataManager.Instance.HandCard.Count);
        for (int i = 0; i < limitCount; i++)
        {
            BattleManager.Instance.ThrowAwayHandCard(i, 0.5f);
        }
        EventManager.Instance.RemoveEventRegister(EventDefinition.eventAttack, EventAttack);
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
