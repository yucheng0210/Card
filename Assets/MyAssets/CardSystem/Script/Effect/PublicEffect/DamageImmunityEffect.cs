using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageImmunityEffect : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        BattleManager.Instance.AddState(BattleManager.Instance.CurrentOnceBattlePositiveList, GetType().Name, value);
    }

    public string SetTitleText()
    {
        return "免傷";
    }
    public string SetDescriptionText()
    {
        return "抵抗一次攻擊。";
    }

}
