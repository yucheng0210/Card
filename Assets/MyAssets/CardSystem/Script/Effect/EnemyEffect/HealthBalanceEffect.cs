using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBalanceEffect : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        EnemyData fromEnemyData = (EnemyData)BattleManager.Instance.IdentifyCharacter(fromLocation);
        EnemyData toEnemyData = (EnemyData)BattleManager.Instance.IdentifyCharacter(toLocation);
        Enemy fromEnemy = fromEnemyData.EnemyTrans.GetComponent<Enemy>();
        Enemy toEnemy = toEnemyData.EnemyTrans.GetComponent<Enemy>();
        Debug.Log(toEnemyData.CurrentHealth);
        if (toEnemyData.CurrentHealth <= 0)
        {
            toEnemy.MyAnimator.SetTrigger("isResurrection");
            toEnemy.IsDeath = false;
        }
        fromEnemyData.CurrentHealth -= value;
        toEnemyData.CurrentHealth += value;
        fromEnemy.TargetLocation = toLocation;
    }

    public string SetTitleText()
    {
        return "生命平衡";
    }
    public string SetDescriptionText()
    {
        return "生命平衡。";
    }

}
