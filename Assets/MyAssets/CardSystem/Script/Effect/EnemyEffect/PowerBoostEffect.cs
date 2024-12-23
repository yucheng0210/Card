using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerBoostEffect : IEffect
{
    private int attackIncreaseCount;
    private EnemyData enemyData;
    private Dictionary<string, EnemyData> enemyList;

    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        enemyList = BattleManager.Instance.CurrentEnemyList;

        if (enemyList.TryGetValue(fromLocation, out enemyData))
        {
            attackIncreaseCount = value;
            enemyData.PassiveSkills.Remove(GetType().Name);
            EventManager.Instance.AddEventRegister(EventDefinition.eventEnemyTurn, enemyData, EventEnemyTurn);
        }
    }

    private void EventEnemyTurn(params object[] args)
    {
        // 獲取攻擊者
        CharacterData attacker = (CharacterData)args[4];

        // 如果攻擊者是目標敵人，增加攻擊力
        if (enemyData == attacker)
        {
            enemyData.CurrentAttack += attackIncreaseCount;
        }
        enemyData.CurrentAttack += attackIncreaseCount;
    }

    public string SetTitleText()
    {
        return "力量增幅";
    }

    public string SetDescriptionText()
    {
        return "每回合提升自身的攻擊力。";
    }
}
