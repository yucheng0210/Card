using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoisonExplosionEffect : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        int damage = BattleManager.Instance.CurrentNegativeState["PoisonedEffect"];
        BattleManager.Instance.TakeDamage(BattleManager.Instance.CurrentEnemyList[fromLocation], BattleManager.Instance.CurrentPlayerData, damage
        , BattleManager.Instance.CurrentLocationID, 0);
        BattleManager.Instance.CurrentNegativeState.Remove("PoisonedEffect");
    }

    public string SetDescriptionText()
    {
        throw new System.NotImplementedException();
    }

    public string SetTitleText()
    {
        throw new System.NotImplementedException();
    }
}
