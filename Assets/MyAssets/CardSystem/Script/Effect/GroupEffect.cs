using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupEffect : IEffect
{
    public void ApplyEffect(int value, string target)
    {
        EnemyData enemyData = BattleManager.Instance.CurrentMinionsList[target];
        int minionsCount = BattleManager.Instance.GetMinionsIDCount(value);
        enemyData.CurrentAttack = enemyData.MinAttack * minionsCount;
        enemyData.DamageReduction = 15 * minionsCount;
    }

    public Sprite SetIcon()
    {
        throw new System.NotImplementedException();
    }

}
