using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightingSpiritEffect : IEffect
{
    public void ApplyEffect(int value, string target)
    {
        string typeName = GetType().Name;
        Dictionary<string, int> currentOnceBattlePositiveList = BattleManager.Instance.CurrentOnceBattlePositiveList;
        if (currentOnceBattlePositiveList.ContainsKey(typeName))
            currentOnceBattlePositiveList[typeName]++;
        else
            currentOnceBattlePositiveList.Add(typeName, 1);
        int damage = 8 * currentOnceBattlePositiveList[typeName];
        BattleManager.Instance.TakeDamage(BattleManager.Instance.CurrentPlayerData, BattleManager.Instance.CurrentEnemyList[target], damage, target, 0);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    public string SetDescriptionText()
    {
        return "根據戰意的層數額外增加傷害。";
    }


    public string SetTitleText()
    {
        return "戰意";
    }
}
