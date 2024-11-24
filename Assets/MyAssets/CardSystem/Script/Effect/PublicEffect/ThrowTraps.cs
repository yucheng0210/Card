using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowTraps : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        EnemyData enemyData = BattleManager.Instance.CurrentEnemyList[fromLocation];
        Enemy enemy = enemyData.EnemyTrans.GetComponent<Enemy>();
        List<string> trapList = enemy.CurrentActionRangeTypeList;
        BattleManager.Instance.AddTrap(trapList, value);
        BattleManager.Instance.CheckPlayerLocationInTrapRange();
    }
    public string SetTitleText()
    {
        return "陷阱投擲。";
    }
    public string SetDescriptionText()
    {
        return "投擲數個陷阱在地圖上。";
    }
    public BattleManager.ActionRangeType SetEffectAttackType() { return BattleManager.ActionRangeType.ThrowScattering; }
}
