using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeopardMechanismEffect : IEffect
{
    private EnemyData enemyData;
    private Enemy enemy;
    private int removeCardCount;
    private int additionAttackCount;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        enemyData = BattleManager.Instance.CurrentEnemyList[fromLocation];
        enemy = enemyData.EnemyTrans.GetComponent<Enemy>();
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
        EventManager.Instance.AddEventRegister(EventDefinition.eventEnemyTurn, EventEnemyTurn);
    }
    private void EventPlayerTurn(params object[] args)
    {
        additionAttackCount = 3;
        enemy.AdditionAttackCount = Mathf.Max(additionAttackCount, 0);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }
    private void EventTakeDamage(params object[] args)
    {
        if (args[4] == enemyData)
        {
            additionAttackCount--;
            enemy.AdditionAttackCount = Mathf.Max(additionAttackCount, 0);
            EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
        }
    }
    private void EventEnemyTurn(params object[] args)
    {
        if (!enemy.InRange && enemy.MyActionType == Enemy.ActionType.Attack)
        {
            removeCardCount++;
            EventManager.Instance.AddEventRegister(EventDefinition.eventAttack, EventAttack);
        }
        else
        {
            removeCardCount = 0;
        }
    }
    private void EventAttack(params object[] args)
    {
        int limitCount = Mathf.Min(removeCardCount, DataManager.Instance.HandCard.Count);
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
