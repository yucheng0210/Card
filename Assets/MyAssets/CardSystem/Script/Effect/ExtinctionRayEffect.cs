using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExtinctionRayEffect : IEffect
{
    public void ApplyEffect(int value, string target)
    {
        int mana = BattleManager.Instance.CurrentPlayerData.Mana;
        int upCount = mana / 10;
        int damage = (int)(mana * 2 + 2 * Mathf.Pow(2, upCount));
        BattleManager.Instance.TakeDamage(BattleManager.Instance.CurrentPlayerData, BattleManager.Instance.CurrentEnemyList[target], damage, target);
        BattleManager.Instance.ConsumeMana(mana);
    }

    public string SetDescriptionText()
    {
        throw new System.NotImplementedException();
    }

    public Sprite SetIcon()
    {
        throw new System.NotImplementedException();
    }

    public string SetTitleText()
    {
        throw new System.NotImplementedException();
    }
}
