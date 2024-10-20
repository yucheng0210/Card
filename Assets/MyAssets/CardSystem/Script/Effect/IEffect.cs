using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IEffect
{
    void ApplyEffect(int value, string target);
    Sprite SetIcon() { return Resources.Load<Sprite>("EffectImage/" + GetType().Name); }
    string SetTitleText();
    string SetDescriptionText();
    string SetPassiveEffectDescriptionText() { return SetDescriptionText(); }
    BattleManager.ActionRangeType SetEffectAttackType() { return BattleManager.ActionRangeType.None; }
}
