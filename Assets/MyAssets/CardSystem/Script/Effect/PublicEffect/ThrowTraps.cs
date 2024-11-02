using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowTraps : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {

    }
    public string SetTitleText()
    {
        throw new System.NotImplementedException();
    }
    public string SetDescriptionText()
    {
        throw new System.NotImplementedException();
    }
    public BattleManager.ActionRangeType SetEffectAttackType() { return BattleManager.ActionRangeType.Throw; }
}
