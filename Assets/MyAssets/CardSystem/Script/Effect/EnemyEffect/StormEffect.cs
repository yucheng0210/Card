using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormEffect : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        //BattleManager.Instance.ThrowAwayHandCard(value, 0.5f);
    }

    public string SetTitleText()
    {
        return "暴風";
    }

    public string SetDescriptionText()
    {
        return "強制丟棄手牌。";
    }
}
