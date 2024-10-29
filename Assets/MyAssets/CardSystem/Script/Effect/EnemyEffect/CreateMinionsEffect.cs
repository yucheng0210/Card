using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateMinionsEffect : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        BattleManager.Instance.AddMinions(2005, value, fromLocation);
    }
    public string SetTitleText()
    {
        return "召喚爪牙";
    }

    public string SetDescriptionText()
    {
        return "召喚數隻爪牙。";
    }

}
