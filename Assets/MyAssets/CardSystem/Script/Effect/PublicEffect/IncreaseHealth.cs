using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IncreaseHealth : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        CharacterData recoverData = BattleManager.Instance.IdentifyCharacter(fromLocation);
        int recoveryAmount = value < 0 ? -value : BattleManager.Instance.GetPercentage(recoverData.MaxHealth, value);
        BattleManager.Instance.Recover(recoverData, recoveryAmount, fromLocation);
        AudioManager.Instance.SEAudio(2);
    }

    public string SetTitleText()
    {
        return "治癒";
    }

    public string SetDescriptionText()
    {
        return "立刻恢復血量。";
    }
}
