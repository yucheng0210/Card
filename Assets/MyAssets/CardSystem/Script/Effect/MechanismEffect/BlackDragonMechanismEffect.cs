using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class BlackDragonMechanismEffect : IEffect
{
    private int maxDamageCount = 200;
    private EnemyData enemyData;
    private Enemy enemy;
    private int stage_2Count = 1;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        enemyData = BattleManager.Instance.CurrentEnemyList[fromLocation];
        enemy = enemyData.EnemyTrans.GetComponent<Enemy>();
        enemyData.DamageLimit = maxDamageCount;
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, enemyData, EventTakeDamage);
    }
    private void SpecialActionStage_2()
    {
        if (stage_2Count == 3)
        {

        }
        int shieldCount = 100 * stage_2Count;
        BattleManager.Instance.TemporaryChangeShield(enemy, shieldCount);
        stage_2Count++;
        enemyData.DamageLimit = maxDamageCount;
    }
    private void EventTakeDamage(params object[] args)
    {
        if (args[5] == enemyData)
        {
            enemyData.DamageLimit -= (int)args[6];
            if (enemyData.DamageLimit <= 0)
            {
                SpecialActionStage_2();
                EffectFactory.Instance.CreateEffect("DizzinessEffect").ApplyEffect(0, "", "");
            }
        }
    }
    public string SetTitleText()
    {
        return "";
    }
    public string SetDescriptionText()
    {
        return "";
    }

}
