using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightingSpiritEffect : IEffect
{
    public void ApplyEffect(int value, string target)
    {
        Dictionary<string, int> currentOnceBattlePositiveList = BattleManager.Instance.CurrentOnceBattlePositiveList;
        if (currentOnceBattlePositiveList.ContainsKey(GetType().Name))
            currentOnceBattlePositiveList[GetType().Name]++;
        else
            currentOnceBattlePositiveList.Add(GetType().Name, 1);
        int damage = 8 * currentOnceBattlePositiveList[GetType().Name];
        BattleManager.Instance.TakeDamage(BattleManager.Instance.CurrentPlayerData, BattleManager.Instance.CurrentEnemyList[target], damage, target);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    public Sprite SetIcon()
    {
        return Resources.Load<Sprite>("EffectImage/FightingSpirit");
    }
}
