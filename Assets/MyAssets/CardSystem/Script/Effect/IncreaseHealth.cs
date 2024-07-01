using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseHealth : IEffect
{
    public void ApplyEffect(int value, string target)
    {
        PlayerData playerData = DataManager.Instance.PlayerList[DataManager.Instance.PlayerID];
        playerData.CurrentHealth += playerData.MaxHealth * value / 100;
    }
    Sprite IEffect.SetIcon()
    {
        throw new System.NotImplementedException();
    }
}