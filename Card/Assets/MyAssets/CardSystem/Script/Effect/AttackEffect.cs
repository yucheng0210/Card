using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffect : IEffect
{
    public void ApplyEffect(int target, int value)
    {
        if (DataManager.Instance.EnemyList[target] != null)
            BattleManager.Instance.TakeDamage(DataManager.Instance.EnemyList[target], value);
        else if (DataManager.Instance.PlayerList[target] != null)
            BattleManager.Instance.TakeDamage(DataManager.Instance.PlayerList[target], value);
    }
}
