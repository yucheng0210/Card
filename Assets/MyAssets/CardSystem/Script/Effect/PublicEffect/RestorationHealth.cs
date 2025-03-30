using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestorationHealth : IEffect
{
    private Dictionary<string, int> currentPositiveState;
    private string typeName;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        typeName = GetType().Name;
        currentPositiveState = BattleManager.Instance.CurrentPositiveState;
        BattleManager.Instance.AddState(currentPositiveState, typeName, value);
        EventManager.Instance.AddEventRegister(EventDefinition.eventEnemyTurn, BattleManager.Instance.CurrentPlayerData, EventEnemyTurn);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }
    private void EventEnemyTurn(params object[] args)
    {
        if (!currentPositiveState.ContainsKey(typeName))
        {
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventEnemyTurn, EventEnemyTurn);
            return;
        }
        BattleManager.Instance.Recover(BattleManager.Instance.CurrentPlayerData, currentPositiveState[typeName], BattleManager.Instance.CurrentPlayerLocation);
    }
    public string SetTitleText()
    {
        return "恢復印記";
    }
    public string SetDescriptionText()
    {
        return "根據層數每回合恢復等量血量。";
    }

}
