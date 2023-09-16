using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CantMoveEffect : IEffect
{
    public void ApplyEffect(int value, string target)
    {
        BattleManager.Instance.CurrentNegativeState.Add(BattleManager.NegativeState.CantMove);
    }
}
