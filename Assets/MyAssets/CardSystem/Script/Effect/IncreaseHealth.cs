using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseHealth : IEffect
{
    public void ApplyEffect(int value, string target)
    {
        CharacterData recoverData;
        if (target == BattleManager.Instance.CurrentLocationID)
            recoverData = BattleManager.Instance.CurrentPlayerData;
        else
            recoverData = BattleManager.Instance.CurrentEnemyList[target];
        BattleManager.Instance.Recover(recoverData, recoverData.MaxHealth * value / 100, BattleManager.Instance.CurrentLocationID);
    }
    Sprite IEffect.SetIcon()
    {
        throw new System.NotImplementedException();
    }
}