using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseManaEffect : IEffect
{
    public void ApplyEffect(int value, int target)
    {
        DataManager.Instance.PlayerList[target].Mana += value;
    }
}
