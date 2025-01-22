using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ApplyBuff : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        Dictionary<string, EnemyData> minionsList = BattleManager.Instance.CurrentMinionsList;
        for (int i = 0; i < minionsList.Count; i++)
        {
            string key = minionsList.ElementAt(i).Key;
            EffectFactory.Instance.CreateEffect(nameof(PowerEffect)).ApplyEffect(value, key, "");
        }
    }

    public string SetTitleText()
    {
        return "賜福";
    }
    public string SetDescriptionText()
    {
        return "給予場上所有爪牙增益。";
    }

}
