using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SniperEffect : IEffect
{
    private EnemyData enemyData;
    private int attackMultiplier;

    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        if (BattleManager.Instance.CurrentEnemyList.TryGetValue(fromLocation, out enemyData))
        {
            attackMultiplier = value;
            EventManager.Instance.AddEventRegister(EventDefinition.eventMove, EventMove);
            EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
            enemyData.PassiveSkills.Remove(GetType().Name);
        }
    }

    private void EventMove(params object[] args)
    {
        RefreshAttack();
    }

    private void EventPlayerTurn(params object[] args)
    {
        RefreshAttack();
    }
    private void RefreshAttack()
    {
        // Assuming args[4] contains the attacking enemy data
        Dictionary<string, EnemyData> currentEnemyList = BattleManager.Instance.CurrentEnemyList;
        if (!currentEnemyList.ContainsValue(enemyData))
        {
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventMove, EventMove);
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
            return;
        }
        string defenderLocation = BattleManager.Instance.GetEnemyKey(enemyData);
        int distance = BattleManager.Instance.GetRoute(defenderLocation, BattleManager.Instance.CurrentLocationID, BattleManager.CheckEmptyType.EnemyAttack).Count;
        enemyData.CurrentAttack = enemyData.MinAttack + Mathf.RoundToInt(enemyData.MinAttack * (distance - 1) * (attackMultiplier / 100f));
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    public string SetTitleText()
    {
        return "地圖炮";
    }

    public string SetDescriptionText()
    {
        return "距離越遠傷害越高。";
    }
}
