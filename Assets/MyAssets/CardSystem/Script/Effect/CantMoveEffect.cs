using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CantMoveEffect : IEffect
{
    public Sprite EffectIcon { get; private set; }
    
    public void ApplyEffect(int value, string target)
    {
        BattleManager.Instance.CurrentNegativeState.Add(BattleManager.NegativeState.CantMove);
    }

    public void SetIcon()
    {
       
    }
}
