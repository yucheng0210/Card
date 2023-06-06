using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseManaEffect : IEffect
{
    private int value;

    public IncreaseManaEffect(int value)
    {
        this.value = value;
    }

    public void ApplyEffect()
    {
        DataManager.Instance.PlayerList[DataManager.Instance.PlayerID].Mana += value;
    }
}
