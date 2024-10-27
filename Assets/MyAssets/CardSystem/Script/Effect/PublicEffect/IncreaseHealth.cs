using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IncreaseHealth : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        // 确定目标数据
        CharacterData recoverData = BattleManager.Instance.IdentifyCharacter(fromLocation);

        // 计算恢复的生命值
        int recoveryAmount = Mathf.RoundToInt(recoverData.MaxHealth * (value / 100f));

        // 执行恢复操作
        BattleManager.Instance.Recover(recoverData, recoveryAmount, fromLocation);
    }

    public string SetTitleText()
    {
        return "治癒";
    }

    public string SetDescriptionText()
    {
        return "回復血量。";
    }
}
