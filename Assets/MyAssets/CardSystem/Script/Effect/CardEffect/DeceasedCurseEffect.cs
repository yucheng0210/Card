using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeceasedCurseEffect : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        BattleManager.Instance.CurrentPlayerData.MaxHealth -= value;
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }
    public string SetTitleText()
    {
        return "亡者低語";
    }
    public string SetDescriptionText()
    {
        return "每回合抽到此卡扣除一點最大生命。";
    }
}
