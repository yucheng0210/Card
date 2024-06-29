using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCardEffect : IEffect
{
    public Sprite EffectIcon { get; private set; }

    public void ApplyEffect(int value, string target)
    {
        EventManager.Instance.DispatchEvent(EventDefinition.eventDrawCard, value);
    }

    public void SetIcon()
    {
        throw new System.NotImplementedException();
    }
}
