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
        currentMana *= BattleManager.Instance.ManaMultiplier;
        BattleManager.Instance.CurrentPlayerData.Mana += currentMana;
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
