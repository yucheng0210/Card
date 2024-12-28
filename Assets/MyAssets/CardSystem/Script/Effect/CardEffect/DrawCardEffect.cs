using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawCardEffect : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        EventManager.Instance.DispatchEvent(EventDefinition.eventDrawCard, value);
    }

    public string SetDescriptionText()
    {
        return "抽卡";
    }

    public string SetTitleText()
    {
        return "抽卡";
    }
}
