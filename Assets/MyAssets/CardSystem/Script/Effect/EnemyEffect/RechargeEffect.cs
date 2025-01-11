using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RechargeEffect : IEffect
{
    private EnemyData enemyData;
    private Enemy enemy;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        enemyData = BattleManager.Instance.CurrentEnemyList[fromLocation];
        enemy = enemyData.EnemyTrans.GetComponent<Enemy>();
        enemy.AdditionAttackMultiplier = value;
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
    }
    private void EventTakeDamage(params object[] args)
    {
        if (args[4] == enemyData)
        {
            enemy.AdditionAttackMultiplier = 1;
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
        }
    }
    public string SetTitleText()
    {
        return "充能";
    }
    public string SetDescriptionText()
    {
        return "強化下一次攻擊。";
    }

}
