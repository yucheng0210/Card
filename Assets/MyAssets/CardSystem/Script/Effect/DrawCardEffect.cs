using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawCardEffect : IEffect
{
    public void ApplyEffect(int value, string target)
    {
        EventManager.Instance.DispatchEvent(EventDefinition.eventDrawCard, value);
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
