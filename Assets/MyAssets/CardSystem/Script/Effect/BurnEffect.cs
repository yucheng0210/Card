using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BurnEffect : IEffect
{
    public void ApplyEffect(int value, string target)
    {
        //BattleManager.Instance.TakeDamage(BattleManager.Instance.CurrentEnemyList[target], 2, target);
    }

    public string SetDescriptionText()
    {
        throw new System.NotImplementedException();
    }

    public string SetTitleText()
    {
        throw new System.NotImplementedException();
    }
}
