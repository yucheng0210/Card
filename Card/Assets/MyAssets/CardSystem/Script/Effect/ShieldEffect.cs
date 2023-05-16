using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEffect : IEffect
{
    public void ApplyEffect(int target, int value)
    {
        if (DataManager.Instance.EnemyList[target] != null)
            BattleManager.Instance.GetShield(DataManager.Instance.EnemyList[target], value);
        else if (DataManager.Instance.PlayerList[target] != null)
            BattleManager.Instance.GetShield(DataManager.Instance.PlayerList[target], value);
    }
}
