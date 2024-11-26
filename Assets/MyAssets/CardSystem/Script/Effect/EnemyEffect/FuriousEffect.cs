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
            EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
        }
    }

    private void EventPlayerTurn(params object[] args)
    {
        if (!enemyList.ContainsValue(enemyData))
        {
            // 移除事件監聽
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
            return;
        }
        enemyData.CurrentAttack += attackIncreaseCount;
    }

    public string SetTitleText()
    {
        return "狂怒";
    }

    public string SetDescriptionText()
    {
        return "每回合提升自身的攻擊力。";
    }
}
