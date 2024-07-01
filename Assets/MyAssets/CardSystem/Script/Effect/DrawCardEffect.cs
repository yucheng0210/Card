using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCardEffect : IEffect
{
    public void ApplyEffect(int value, string target)
    {
        EventManager.Instance.DispatchEvent(EventDefinition.eventDrawCard, value);
    }

    Sprite IEffect.SetIcon()
    {
        throw new System.NotImplementedException();
    }
}
