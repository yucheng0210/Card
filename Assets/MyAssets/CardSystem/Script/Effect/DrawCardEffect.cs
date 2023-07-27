using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCardEffect : IEffect
{
    public void ApplyEffect(int value, int target)
    {
        EventManager.Instance.DispatchEvent(EventDefinition.eventDrawCard, value);
    }
}
