using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TauntEffect : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        BattleManager.Instance.CurrentNegativeState.Add(GetType().Name, value);
        EffectFactory.Instance.CreateEffect(nameof(CantMoveEffect)).ApplyEffect(value, fromLocation, toLocation);
    }

    public string SetTitleText()
    {
        return "";
    }
    public string SetDescriptionText()
    {
        return "";
    }
    public  BattleManager.ActionRangeType SetEffectAttackType() { return BattleManager.ActionRangeType.Surrounding; }
}
