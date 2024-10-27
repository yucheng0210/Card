using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateMinionsEffect : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        int minionsCount = Mathf.Clamp(value - BattleManager.Instance.CurrentMinionsList.Count, 0, value);
        BattleManager.Instance.AddMinions(2005, minionsCount, fromLocation);
    }
    public string SetTitleText()
    {
        return "召喚爪牙";
    }

    public string SetDescriptionText()
    {
        return "召喚數隻弱化版的本體爪牙。"; 
    }

}
