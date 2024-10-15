using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupEffect : IEffect
{
    private string leaderLocation;
    private int minionsID;
    public void ApplyEffect(int value, string target)
    {
        leaderLocation = target;
        minionsID = value;
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
    }
    private void EventTakeDamage(params object[] args)
    {
        if (!BattleManager.Instance.CurrentEnemyList.ContainsKey(leaderLocation))
            return;
        EnemyData enemyData = BattleManager.Instance.CurrentEnemyList[leaderLocation];
        int minionsCount = BattleManager.Instance.GetMinionsIDCount(minionsID);
        enemyData.CurrentAttack = enemyData.MinAttack * (1 + minionsCount);
        enemyData.DamageReduction = 15 * (1 + minionsCount);
    }

    public string SetTitleText()
    {
        return "族群效應";
    }

    public string SetDescriptionText()
    {
        return "場上每多一隻爪牙減傷和攻擊上升";
    }
}