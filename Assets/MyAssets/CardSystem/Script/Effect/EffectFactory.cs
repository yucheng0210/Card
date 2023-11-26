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
            case "IncreaseHealth":
                return new IncreaseHealth();
            case "FireBallEffect":
                return new FireBallEffect();
            case "DrawCardEffect":
                return new DrawCardEffect();
            case "MoveEffect":
                return new MoveEffect();
            case "BurnEffect":
                return new BurnEffect();
            case "CantMoveEffect":
                return new CantMoveEffect();
            default:
                return null;
        }
    }
}
