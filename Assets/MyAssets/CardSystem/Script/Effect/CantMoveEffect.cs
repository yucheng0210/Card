using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CantMoveEffect : IEffect
{

    public void ApplyEffect(int value, string target)
    {
        if (BattleManager.Instance.CurrentNegativeState.ContainsKey(GetType().Name))
            BattleManager.Instance.CurrentNegativeState[GetType().Name] += value;
        else
            BattleManager.Instance.CurrentNegativeState.Add(GetType().Name, value);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    public Sprite SetIcon()
    {
        return Resources.Load<Sprite>("EffectImage/CantMove");
    }
}
