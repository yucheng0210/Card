using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DizzinessEffect : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Enemy);
    }

    public string SetTitleText()
    {
        return "暈眩";
    }

    public string SetDescriptionText()
    {
        return "強制下一回合。";
    }
}
