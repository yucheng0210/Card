using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CantMoveEffect : IEffect
{
    private Dictionary<string, int> negativeState = BattleManager.Instance.CurrentNegativeState;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        string effectName = GetType().Name;
        BattleManager.Instance.AddState(negativeState, effectName, value);
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
