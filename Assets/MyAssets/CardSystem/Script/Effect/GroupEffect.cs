using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        EnemyData enemyData = BattleManager.Instance.CurrentEnemyList[leaderLocation];
        int minionsCount = BattleManager.Instance.GetMinionsIDCount(minionsID);
        enemyData.CurrentAttack = enemyData.MinAttack * (1 + minionsCount);
        enemyData.DamageReduction = 15 * (1 + minionsCount);
    }
    public Sprite SetIcon()
    {
        return Resources.Load<Sprite>("EffectImage/CantMove");
    }

}