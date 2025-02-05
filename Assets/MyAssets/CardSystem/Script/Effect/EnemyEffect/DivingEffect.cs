using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivingEffect : IEffect
{
    private EnemyData enemyData;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        enemyData = BattleManager.Instance.CurrentEnemyList[fromLocation];
        enemyData.DamageReduction = 100;
        enemyData.PassiveSkills.Remove(GetType().Name);
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, enemyData, EventTakeDamage);
        EventManager.Instance.AddEventRegister(EventDefinition.eventEnemyTurn, enemyData, EventEnemyTurn);
    }
    private void EventTakeDamage(params object[] args)
    {
        if ((CharacterData)args[4] != enemyData)
        {
            return;
        }
        enemyData.DamageReduction = 0;
    }
    private void EventEnemyTurn(params object[] args)
    {
        enemyData.DamageReduction = 100;
    }
    public string SetTitleText()
    {
        return "潛水";
    }
    public string SetDescriptionText()
    {
        return "回合開始進入潛水狀態，免疫所有傷害，發動攻擊則會退出狀態，並在下回合恢復。";
    }

}
