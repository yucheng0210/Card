using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowTraps : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        BattleManager.CheckEmptyType checkEmptyType = BattleManager.CheckEmptyType.Move;
        BattleManager.ActionRangeType actionRangeType = BattleManager.ActionRangeType.Surrounding;
        List<string> trapList = BattleManager.Instance.GetActionRangeTypeList(fromLocation, 5, checkEmptyType, actionRangeType);
        BattleManager.Instance.AddTrap(trapList, value);
        BattleManager.Instance.CheckPlayerLocationInTrapRange();
        BattleManager.Instance.RefreshCheckerboardList();
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
