using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallEffect : IEffect
{
    public void ApplyEffect()
    {
        int mana = DataManager.Instance.PlayerList[DataManager.Instance.PlayerID].Mana;
        int upCount = mana / 10;
        int damage = (int)(mana * 2 + 2 * Mathf.Pow(2, upCount));
        BattleManager.Instance.TakeDamage(DataManager.Instance.EnemyList[1001], damage);
    }
}
