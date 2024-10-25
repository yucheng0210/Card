using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDownEffect : IEffect
{
    public void ApplyEffect(int value, string target)
    {
        BattleManager.Instance.PlayerMoveCount -= value;
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    public string SetTitleText()
    {
        return "減速波";
    }
    public string SetDescriptionText()
    {
        return "扣除一格移動點。";
    }
    public BattleManager.ActionRangeType SetEffectAttackType()
    {
        return BattleManager.ActionRangeType.Surrounding;
    }
}
