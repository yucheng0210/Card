using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDamageReductionEffect : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        BattleManager.Instance.CurrentPlayerData.DamageReduction = value;
    }
    public string SetTitleText()
    {
        return "提升減傷率";
    }
    public string SetDescriptionText()
    {
        return "提升減傷率。";
    }

}
