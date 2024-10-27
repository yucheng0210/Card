using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDownEffect : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        BattleManager.Instance.CurrentNegativeState.Add(GetType().Name, value);
        BattleManager.Instance.PlayerOnceMoveConsume = 2;
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    public string SetTitleText()
    {
        return "減速波";
    }
    public string SetDescriptionText()
    {
        return "移動需要耗費更多點數。";
    }
    public BattleManager.ActionRangeType SetEffectAttackType()
    {
        return BattleManager.ActionRangeType.Surrounding;
    }
}
