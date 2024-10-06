using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public string SetDescriptionText()
    {
        throw new System.NotImplementedException();
    }
    public string SetTitleText()
    {
        throw new System.NotImplementedException();
    }
}
