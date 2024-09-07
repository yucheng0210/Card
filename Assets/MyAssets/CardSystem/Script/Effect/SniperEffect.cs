using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperEffect : IEffect
{
    private EnemyData enemyData;
    private int attackMultiplier;
    public void ApplyEffect(int value, string target)
    {
        enemyData = BattleManager.Instance.CurrentEnemyList[target];
        attackMultiplier = value;
        EventManager.Instance.AddEventRegister(EventDefinition.eventEnemyTurn, EventEnemyTurn);
        enemyData.PassiveSkills.Remove(GetType().Name);
    }
    private void EventEnemyTurn(params object[] args)
    {
        EnemyData attacker = (EnemyData)args[4];
        if (enemyData == attacker)
        {
            string defenderLocation = attacker.EnemyTrans.GetComponent<Enemy>().EnemyLocation;
            int distance = (int)BattleManager.Instance.GetDistance(defenderLocation);
            attacker.CurrentAttack = attacker.MinAttack + attacker.MinAttack * (distance - 1) * attackMultiplier;
        }
    }
    public Sprite SetIcon()
    {
        throw new System.NotImplementedException();
    }
}
