using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectFactory : IEffectFactory
{
    public IEffect CreateEffect(string effectType)
    {
        switch (effectType)
        {
            case "AttackEffect":
                return new AttackEffect();
            case "ShieldEffect":
                return new ShieldEffect();
            default:
                return null;
        }
    }
}
