using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IEffect
{
    void ApplyEffect(int value, string fromLocation, string toLocation);
    Sprite SetIcon() { return Resources.Load<Sprite>("EffectImage/" + GetType().Name); }
    string SetPassiveEffectDescriptionText() { return SetDescriptionText(); }
    BattleManager.ActionRangeType SetEffectAttackType() { return BattleManager.ActionRangeType.None; }
    int SetEffectRange() { return -1; }
    string SetTitleText();
    string SetDescriptionText();
}
