using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CantMoveEffect : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        var negativeState = BattleManager.Instance.CurrentNegativeState;
        string effectName = GetType().Name;
        negativeState[effectName] = negativeState.ContainsKey(effectName) ? negativeState[effectName] + value : value;
        // 發送 UI 更新事件
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    public string SetTitleText()
    {
        return "禁錮";
    }
    public string SetDescriptionText()
    {
        return "封印移動。";
    }

    public BattleManager.ActionRangeType SetEffectAttackType()
    {
        return BattleManager.ActionRangeType.Cone;
    }
}
