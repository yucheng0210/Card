using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharmEffect : IEffect
{
    private int charmCount;
    private string typeName;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        typeName = GetType().Name;
        charmCount = value;
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
        EventManager.Instance.AddEventRegister(EventDefinition.eventEnemyTurn, EventEnemyTurn);
    }
    private void EventPlayerTurn(params object[] args)
    {
        BattleManager.Instance.CurrentNegativeState.Add(typeName, charmCount);
    }
    private void EventEnemyTurn(params object[] args)
    {
        BattleManager.Instance.CurrentNegativeState.Remove(typeName);
    }
    public string SetTitleText()
    {
        return "魅惑";
    }
    public string SetDescriptionText()
    {
        return "無效第一張出牌。";
    }

}
