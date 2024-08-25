using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseManaEffect : IEffect
{
    public void ApplyEffect(int value, string target)
    {
        int currentMana = value;
        if (value == -1)
            currentMana = BattleManager.Instance.CurrentConsumeMana;
        currentMana *= BattleManager.Instance.ManaMultiplier;
        BattleManager.Instance.CurrentPlayerData.Mana += currentMana;
    }

    Sprite IEffect.SetIcon()
    {
        throw new System.NotImplementedException();
    }
}
