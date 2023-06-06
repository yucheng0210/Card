using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffectFactory
{
    IEffect CreateEffect(string effectType, int value);
}
