using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectFactory : Singleton<EffectFactory>
{
    public IEffect CreateEffect(string effectType)
    {
        switch (effectType)
        {
            case "IncreaseManaEffect":
                return new IncreaseManaEffect();
            case "FireBallEffect":
                return new FireBallEffect();
            case "DrawCardEffect":
                return new DrawCardEffect();
            default:
                return null;
        }
    }
}
