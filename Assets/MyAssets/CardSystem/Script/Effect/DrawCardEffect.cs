using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCardEffect : IEffect
{
    public void ApplyEffect(int value, int target)
    {
        BattleManager.Instance.AddHandCard(value);
    }
}
