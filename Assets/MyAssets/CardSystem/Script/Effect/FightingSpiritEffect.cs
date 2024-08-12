using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightingSpiritEffect : IEffect
{
    public void ApplyEffect(int value, string target)
    {
        int damage = 8 * (BattleManager.Instance.CurrentFightingSpiritCount + 1);
        BattleManager.Instance.TakeDamage(BattleManager.Instance.CurrentEnemyList[target], damage, target);
        BattleManager.Instance.CurrentFightingSpiritCount++;
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    public Sprite SetIcon()
    {
        throw new System.NotImplementedException();
    }
}
