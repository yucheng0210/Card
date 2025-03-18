using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseMoneyEffect : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        DataManager.Instance.MoneyCount += value;
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    public string SetTitleText()
    {
        throw new System.NotImplementedException();
    }

    public string SetDescriptionText()
    {
        throw new System.NotImplementedException();
    }
}
