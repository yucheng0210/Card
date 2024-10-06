using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateMinionsEffect : IEffect
{
    public void ApplyEffect(int value, string target)
    {
        int minionsCount = Mathf.Clamp(value - BattleManager.Instance.CurrentMinionsList.Count, 0, value);
        BattleManager.Instance.AddMinions(2005, minionsCount, target);
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
