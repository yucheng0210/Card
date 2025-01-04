using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeZoneEffect : IEffect
{
    private EnemyData enemyData;
    private List<string> safeZoneList = new List<string>();
    private int safeZoneCount = 6;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        enemyData = BattleManager.Instance.CurrentEnemyList[fromLocation];
        enemyData.PassiveSkills.Remove(GetType().Name);
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
    }
    private void EventPlayerTurn(params object[] args)
    {
        BattleManager.CheckEmptyType checkEmptyType = BattleManager.CheckEmptyType.Move;
        BattleManager.ActionRangeType actionRangeType = BattleManager.ActionRangeType.AllZone;
        List<string> emptyPlaceList = BattleManager.Instance.GetActionRangeTypeList("", 0, checkEmptyType, actionRangeType);
        for (int i = 0; i < safeZoneCount; i++)
        {
            int randomIndex = Random.Range(0, emptyPlaceList.Count);
            safeZoneList.Add(emptyPlaceList[randomIndex]);
            emptyPlaceList.RemoveAt(randomIndex);
        }
    }
    public string SetTitleText()
    {
        return "";
    }
    public string SetDescriptionText()
    {
        return "";
    }

}
