using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonExplosionEffect : IEffect
{
    public void ApplyEffect(int value, string target)
    {
        int damage = BattleManager.Instance.CurrentNegativeState["PoisonedEffect"];
        BattleManager.Instance.TakeDamage(BattleManager.Instance.CurrentEnemyList[target], BattleManager.Instance.CurrentPlayerData, damage
        , BattleManager.Instance.CurrentLocationID);
        BattleManager.Instance.CurrentNegativeState.Remove("PoisonedEffect");
    }

    public Sprite SetIcon()
    {
        throw new System.NotImplementedException();
    }
}
