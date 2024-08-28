using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuriousEffect : IEffect
{
    public void ApplyEffect(int value, string target)
    {
        EnemyData enemyData = BattleManager.Instance.CurrentEnemyList[target];
        if (enemyData.CurrentHealth <= enemyData.MaxHealth / 2)
            enemyData.CurrentAttack = enemyData.MaxAttack * 2;
    }

    public Sprite SetIcon()
    {
        throw new System.NotImplementedException();
    }

}
