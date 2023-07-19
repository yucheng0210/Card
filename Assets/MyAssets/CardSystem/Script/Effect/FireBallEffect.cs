using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallEffect : IEffect
{
    public void ApplyEffect(int value, int target)
    {
        int mana = DataManager.Instance.PlayerList[DataManager.Instance.PlayerID].Mana;
        int upCount = mana / 10;
        int damage = (int)(mana * 2 + 2 * Mathf.Pow(2, upCount));
        BattleManager.Instance.TakeDamage(BattleManager.Instance.CurrentEnemyList[target], damage);
    }
}
