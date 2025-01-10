using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackDragonMechanismEffect : IEffect
{
    private int maxDamageCount = 200;
    private EnemyData enemyData;
    private Enemy enemy;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        enemyData = BattleManager.Instance.CurrentEnemyList[fromLocation];
        enemy = enemyData.EnemyTrans.GetComponent<Enemy>();
        enemyData.DamageLimit = maxDamageCount;
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, enemyData, EventTakeDamage);
    }
    private void SpecialActionStage_2()
    {
       /* List<(string, int)> actionOrder = new List<(string, int)>();
        actionOrder.Add(("Shield", 300));
        actionOrder.Add(("AllZone", enemyData.CurrentShield));
        enemyData.CurrentAttackOrderIndex = 0;
        enemyData.CurrentAttackOrderStrs = actionOrder;*/
    }
    private void EventTakeDamage(params object[] args)
    {
        if (args[5] == enemyData)
        {
            enemyData.DamageLimit -= (int)args[6];
            if (enemyData.DamageLimit <= 0)
            {
                BattleManager.Instance.TemporaryChangeShield(enemy, 200);
                SpecialActionStage_2();
                EffectFactory.Instance.CreateEffect("DizzinessEffect").ApplyEffect(0, "", "");
                enemyData.DamageLimit = maxDamageCount;
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
