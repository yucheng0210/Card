using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveMarkEffect : IEffect
{
    private EnemyData enemyData;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        enemyData = BattleManager.Instance.CurrentEnemyList[fromLocation];
        if (value == -1)
        {
            EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
        }
        else
        {
            AddMark();
        }

    }
    private void EventTakeDamage(params object[] args)
    {
        if (args[4] == enemyData)
        {
            AddMark();
        }
    }
    private void AddMark()
    {
        if (BattleManager.Instance.CurrentOnceBattlePositiveList.ContainsKey(GetType().Name))
        {
            BattleManager.Instance.CurrentOnceBattlePositiveList[GetType().Name]++;
        }
        else
        {
            BattleManager.Instance.CurrentOnceBattlePositiveList.Add(GetType().Name, 1);
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
