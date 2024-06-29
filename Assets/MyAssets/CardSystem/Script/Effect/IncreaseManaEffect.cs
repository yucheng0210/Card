using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseManaEffect : IEffect
{
    public Sprite EffectIcon { get; private set; }

    public void ApplyEffect(int value, string target)
    {
        DataManager.Instance.PlayerList[DataManager.Instance.PlayerID].Mana += value;
    }

    public void SetIcon()
    {
        throw new System.NotImplementedException();
    }
}
