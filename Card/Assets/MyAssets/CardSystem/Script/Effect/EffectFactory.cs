using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectFactory : IEffectFactory
{
    public IEffect CreateEffect(string effectType, int value)
    {
        switch (effectType)
        {
            case "IncreaseManaEffect":
                return new IncreaseManaEffect(value);
            case "FireBallEffect":
                return new FireBallEffect();
            default:
                return null;
        }
    }
}
