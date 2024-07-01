using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseManaEffect : IEffect
{
    public void ApplyEffect(int value, string target)
    {
        DataManager.Instance.PlayerList[DataManager.Instance.PlayerID].Mana += value;
    }

    Sprite IEffect.SetIcon()
    {
        throw new System.NotImplementedException();
    }
}
