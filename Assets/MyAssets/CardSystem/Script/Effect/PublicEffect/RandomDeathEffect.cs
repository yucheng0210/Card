using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDeathEffect : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        int randomIndex = Random.Range(0, 100);
        if (value >= randomIndex)
        {
            BattleManager.Instance.TakeDamage(BattleManager.Instance.CurrentPlayerData, BattleManager.Instance.CurrentPlayerData, 999, BattleManager.Instance.CurrentPlayerLocation, 5f);
        }
    }

    public string SetTitleText()
    {
        return "死亡之骰";
    }
    public string SetDescriptionText()
    {
        return "特定機率死亡。";
    }
}
