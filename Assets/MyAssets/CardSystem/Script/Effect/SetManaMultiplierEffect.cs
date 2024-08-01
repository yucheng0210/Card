using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetManaMultiplierEffect : IEffect
{
    public void ApplyEffect(int value, string target)
    {
        BattleManager.Instance.ManaMultiplier = value;
    }

    public Sprite SetIcon()
    {
        throw new System.NotImplementedException();
    }
}
