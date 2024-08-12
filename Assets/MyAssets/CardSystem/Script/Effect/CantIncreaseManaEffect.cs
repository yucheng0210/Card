using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CantIncreaseManaEffect : IEffect
{
    public void ApplyEffect(int value, string target)
    {
        if (BattleManager.Instance.CurrentNegativeState.ContainsKey(GetType().Name))
            BattleManager.Instance.CurrentNegativeState[GetType().Name]++;
        else
            BattleManager.Instance.CurrentNegativeState.Add(GetType().Name, 1);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    public Sprite SetIcon()
    {
        throw new System.NotImplementedException();
    }
}
