using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEffect : IEffect
{
    private EnemyData enemyData;
    private Enemy enemy;
    private string typeName;
    private int reciprocalCount = 2;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        typeName = GetType().Name;
        enemyData = BattleManager.Instance.CurrentEnemyList[fromLocation];
        enemyData.DodgeChance = value;
        enemy = enemyData.EnemyTrans.GetComponent<Enemy>();
        enemy.EnemyOnceBattlePositiveList.Add(typeName, reciprocalCount);
        EventManager.Instance.AddEventRegister(EventDefinition.eventEnemyTurn, enemyData, EventEnemyTurn);
    }
    private void EventEnemyTurn(params object[] args)
    {
        enemy.EnemyOnceBattlePositiveList[typeName]--;
        if (enemy.EnemyOnceBattlePositiveList[typeName] == 0)
        {
            enemy.EnemyOnceBattlePositiveList.Remove(typeName);
            enemyData.DodgeChance = 0;
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventEnemyTurn, EventEnemyTurn);
        }
    }
    public string SetTitleText()
    {
        return "飛行";
    }
    public string SetDescriptionText()
    {
        return "獲得25%的閃避機率。";
    }

}
