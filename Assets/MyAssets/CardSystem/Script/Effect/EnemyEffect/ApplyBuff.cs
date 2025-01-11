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
            Enemy enemy = minionsList[key].EnemyTrans.GetComponent<Enemy>();
            enemy.AdditionPower += value;
        }
    }

    public string SetTitleText()
    {
        return "爪牙增益";
    }
    public string SetDescriptionText()
    {
        return "給予場上所有爪牙增益。";
    }

}
