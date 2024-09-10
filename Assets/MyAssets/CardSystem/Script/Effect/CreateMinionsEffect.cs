using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMinionsEffect : IEffect
{
    public void ApplyEffect(int value, string target)
    {
        int minionsCount = Mathf.Clamp(value - BattleManager.Instance.CurrentMinionsList.Count, 0, value);
        BattleManager.Instance.AddMinions(2001, minionsCount, target);
    }

    public Sprite SetIcon()
    {
        throw new System.NotImplementedException();
    }
}
