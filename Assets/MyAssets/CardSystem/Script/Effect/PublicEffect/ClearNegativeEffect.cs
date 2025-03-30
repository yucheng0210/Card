using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearNegativeEffect : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        BattleManager.Instance.CurrentNegativeState.Clear();
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    public string SetTitleText()
    {
        return "消除負面效果";
    }
    public string SetDescriptionText()
    {
        return "消除負面效果。";
    }

}
