using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnEffect : IEffect
{
    public Sprite EffectIcon { get; private set; }

    public void ApplyEffect(int value, string target)
    {
        BattleManager.Instance.TakeDamage(BattleManager.Instance.CurrentEnemyList[target], 2, target);
    }

    public void SetIcon()
    {
        throw new System.NotImplementedException();
    }
}
