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
            minionsList[key].CurrentAttack += value;
        }
    }

    public string SetTitleText()
    {
        throw new System.NotImplementedException();
    }
    public string SetDescriptionText()
    {
        throw new System.NotImplementedException();
    }

}
