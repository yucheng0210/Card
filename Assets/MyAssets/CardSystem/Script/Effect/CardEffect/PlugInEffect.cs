using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlugInEffect : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        for (int i = 0; i < BattleManager.Instance.CurrentEnemyList.Count; i++)
        {
            CharacterData characterData = BattleManager.Instance.CurrentEnemyList.ElementAt(i).Value;
            BattleManager.Instance.TakeDamage(BattleManager.Instance.CurrentPlayerData, characterData, 999, BattleManager.Instance.CurrentEnemyList.ElementAt(i).Key, 0);
        }
    }

    public string SetTitleText()
    {
        return "給我死";
    }
    public string SetDescriptionText()
    {
        return "全部給我死。";
    }

}
