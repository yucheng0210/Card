using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnEffect : IEffect
{
    public void ApplyEffect(int value, string target)
    {
        BattleManager.Instance.TakeDamage(BattleManager.Instance.CurrentEnemyList[target], 2, target);
    }
}
