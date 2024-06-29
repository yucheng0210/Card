using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallEffect : IEffect
{
    public Sprite EffectIcon { get; private set; }

    public void ApplyEffect(int value, string target)
    {
        int mana = DataManager.Instance.PlayerList[DataManager.Instance.PlayerID].Mana;
        int upCount = mana / 10;
        int damage = (int)(mana * 2 + 2 * Mathf.Pow(2, upCount));
        BattleManager.Instance.TakeDamage(BattleManager.Instance.CurrentEnemyList[target], damage, target);
        BattleManager.Instance.ConsumeMana(mana);
    }

    public void SetIcon()
    {
        throw new System.NotImplementedException();
    }
}
