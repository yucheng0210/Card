using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetManaMultiplierEffect : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        BattleManager.Instance.ManaMultiplier = value;
    }

    public string SetDescriptionText()
    {
        return "增幅";
    }

    public string SetTitleText()
    {
        return "";
    }
}
