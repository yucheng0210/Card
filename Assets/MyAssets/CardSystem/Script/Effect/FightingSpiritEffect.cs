using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightingSpiritEffect : IEffect
{
    public void ApplyEffect(int value, string target)
    {
        if (BattleManager.Instance.CurrentOnceBattlePositiveList.ContainsKey(GetType().Name))
            BattleManager.Instance.CurrentOnceBattlePositiveList[GetType().Name]++;
        else
            BattleManager.Instance.CurrentOnceBattlePositiveList.Add(GetType().Name, 1);
        int damage = 8 * BattleManager.Instance.CurrentOnceBattlePositiveList[GetType().Name];
        BattleManager.Instance.TakeDamage(BattleManager.Instance.CurrentEnemyList[target], damage, target);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    public Sprite SetIcon()
    {
        return Resources.Load<Sprite>("EffectImage/FightingSpirit");
    }
}
