using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IncreaseManaEffect : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        int currentMana = value;
        if (value == -1)
        {
            currentMana = BattleManager.Instance.CurrentConsumeMana;
        }
        else if (value == -2)
        {
            currentMana = DataManager.Instance.HandCard.Count * 3;
        }
        currentMana *= BattleManager.Instance.ManaMultiplier;
        BattleManager.Instance.CurrentPlayerData.Mana += currentMana;
    }

    public string SetDescriptionText()
    {
        return "增魔";
    }

    public string SetTitleText()
    {
        return "增加魔力。";
    }
}
