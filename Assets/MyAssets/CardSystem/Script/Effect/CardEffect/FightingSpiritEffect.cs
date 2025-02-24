using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightingSpiritEffect : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        string typeName = GetType().Name;
        Dictionary<string, int> currentOnceBattlePositiveList = BattleManager.Instance.CurrentOnceBattlePositiveList;
        BattleManager.Instance.AddState(currentOnceBattlePositiveList, typeName, 8);
        int damage = currentOnceBattlePositiveList[typeName];
        EnemyData enemyData = (EnemyData)BattleManager.Instance.IdentifyCharacter(toLocation);
        BattleManager.Instance.TakeDamage(BattleManager.Instance.CurrentPlayerData, enemyData, damage, toLocation, 0);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    public string SetTitleText()
    {
        return "戰意";
    }

    public string SetDescriptionText()
    {
        return "根據戰意的層數額外增加傷害。";
    }

}
