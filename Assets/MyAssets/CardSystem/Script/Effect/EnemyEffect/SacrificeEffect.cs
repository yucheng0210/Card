using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacrificeEffect : IEffect
{
    private EnemyData enemyData;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        if (!BattleManager.Instance.CurrentEnemyList.TryGetValue(fromLocation, out enemyData))
        {
            return;
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
