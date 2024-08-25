using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseHealth : IEffect
{
    public void ApplyEffect(int value, string target)
    {
        PlayerData playerData = BattleManager.Instance.CurrentPlayerData;
        BattleManager.Instance.Recover(playerData, playerData.MaxHealth * value / 100, BattleManager.Instance.CurrentLocationID);
    }
    Sprite IEffect.SetIcon()
    {
        throw new System.NotImplementedException();
    }
}