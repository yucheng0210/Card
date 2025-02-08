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
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, enemyData, EventPlayerTurn);
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, enemyData, EventTakeDamage);
        EventManager.Instance.AddEventRegister(EventDefinition.eventEnemyTurn, enemyData, EventEnemyTurn);
        EventPlayerTurn();
        enemyData.MaxPassiveSkillsList.Remove(GetType().Name);
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
        return "每回合獲得兩次額外攻擊，受到攻擊時減少，如果攻擊未命中敵人，則強制丟棄玩家手牌。";
    }
}
