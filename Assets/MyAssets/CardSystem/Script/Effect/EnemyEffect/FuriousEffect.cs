using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuriousEffect : IEffect
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
            EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, enemyData, EventTakeDamage);
        }
    }

    private void EventTakeDamage(params object[] args)
    {
        // 獲取攻擊者
        CharacterData attacker = (CharacterData)args[4];
        Enemy enemy = enemyData.EnemyTrans.GetComponent<Enemy>();
        // 如果攻擊者是目標敵人，增加攻擊力
        if (enemyData == attacker)
        {
            enemy.AdditionPower += attackIncreaseCount;
        }
    }

    public string SetTitleText()
    {
        return "狂怒";
    }

    public string SetDescriptionText()
    {
        return "每次命中敵人提升攻擊力。";
    }
}
