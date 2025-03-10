using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupEffect : IEffect
{
    private string leaderLocation;
    private int minionsID;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        leaderLocation = fromLocation;
        minionsID = value;
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
    }
    private void EventPlayerTurn(params object[] args)
    {
        if (!BattleManager.Instance.CurrentEnemyList.ContainsKey(leaderLocation))
        {
            return;
        }
        EnemyData enemyData = BattleManager.Instance.CurrentEnemyList[leaderLocation];
        int minionsCount = BattleManager.Instance.GetMinionsIDCount(minionsID);
        enemyData.DamageReduction = 15 * (1 + minionsCount);
    }
    public bool IsShowEffectCount() { return false; }
    public string SetTitleText()
    {
        return "族群韌性";
    }

    public string SetDescriptionText()
    {
        return "場上每多一隻爪牙減傷上升。";
    }
}