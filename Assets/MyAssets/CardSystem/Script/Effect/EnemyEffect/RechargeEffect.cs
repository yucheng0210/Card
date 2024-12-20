using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RechargeEffect : IEffect
{
    private EnemyData enemyData;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        enemyData = BattleManager.Instance.CurrentEnemyList[fromLocation];
        enemyData.CurrentAttack += Mathf.RoundToInt(enemyData.MinAttack * (value / 100f));
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
    }
    private void EventTakeDamage(params object[] args)
    {
        if (args[4] == enemyData)
        {
            enemyData.CurrentAttack = enemyData.MinAttack;
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
