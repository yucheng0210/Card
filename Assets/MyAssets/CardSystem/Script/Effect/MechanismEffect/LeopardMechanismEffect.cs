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
        enemy.EnemyOnceBattlePositiveList.Add(GetType().Name, 1);
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, enemyData, EventPlayerTurn);
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, enemyData, EventTakeDamage);
        EventManager.Instance.AddEventRegister(EventDefinition.eventEnemyTurn, enemyData, EventEnemyTurn);
        EventPlayerTurn();
    }
    private void EventPlayerTurn(params object[] args)
    {
        additionAttackCount = 2;
        enemy.AdditionAttackCount = Mathf.Max(additionAttackCount, 0);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }
    private void EventTakeDamage(params object[] args)
    {
        if (args[4] == BattleManager.Instance.CurrentPlayerData && BattleManager.Instance.MyBattleType == BattleManager.BattleType.Attack)
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
        return "沙塵領域";
    }

    public string SetDescriptionText()
    {
        return "玩家每回合重置敵人額外攻擊次數為2次，攻擊敵人可減少次數。若敵人無法有效攻擊，累積丟牌計數並在攻擊時觸發丟牌。";
    }
}
