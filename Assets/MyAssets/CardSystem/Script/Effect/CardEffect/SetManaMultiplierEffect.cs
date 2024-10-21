using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetManaMultiplierEffect : IEffect
{
    public void ApplyEffect(int value, string target)
    {
        BattleManager.Instance.ManaMultiplier = value;
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
