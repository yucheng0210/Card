using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowTraps : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        EnemyData enemyData = BattleManager.Instance.CurrentEnemyList[fromLocation];
        BattleManager.CheckEmptyType checkEmptyType = BattleManager.CheckEmptyType.EnemyAttack;
        List<string> trapList = BattleManager.Instance.GetActionRangeTypeList(fromLocation, enemyData.AttackDistance, checkEmptyType, SetEffectAttackType());
        BattleManager.Instance.AddTrap(trapList, value);
    }
    public string SetTitleText()
    {
        throw new System.NotImplementedException();
    }
    public string SetDescriptionText()
    {
        throw new System.NotImplementedException();
    }
    public BattleManager.ActionRangeType SetEffectAttackType() { return BattleManager.ActionRangeType.ThrowScattering; }
}
