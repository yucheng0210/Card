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
            attackIncreaseCount = Mathf.RoundToInt(enemyData.CurrentAttack * (value / 100f));
            EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
        }
    }

    private void EventTakeDamage(params object[] args)
    {
        if (!enemyList.ContainsValue(enemyData))
        {
            // 移除事件監聽
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
            return;
        }
        if (args.Length < 5)
        {
            return;
        }
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
        return "狂怒";
    }

    public string SetDescriptionText()
    {
        return "每次擊中敵人提升自身的攻擊力。";
    }
}
