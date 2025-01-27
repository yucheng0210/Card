using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SniperEffect : IEffect
{
    private EnemyData enemyData;
    private Enemy enemy;

    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        if (BattleManager.Instance.CurrentEnemyList.TryGetValue(fromLocation, out enemyData))
        {
            enemy = enemyData.EnemyTrans.GetComponent<Enemy>();
            EventManager.Instance.AddEventRegister(EventDefinition.eventMove, enemyData, EventMove);
            EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, enemyData, EventPlayerTurn);
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
        string defenderLocation = BattleManager.Instance.GetEnemyKey(enemyData);
        int distance = BattleManager.Instance.GetRoute(defenderLocation, BattleManager.Instance.CurrentPlayerLocation, BattleManager.CheckEmptyType.EnemyAttack).Count;
        enemy.AdditionAttackMultiplier = distance - 1;
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }
    public bool IsShowEffectCount() { return false; }
    public string SetTitleText()
    {
        return "地圖砲";
    }

    public string SetDescriptionText()
    {
        return "距離越遠傷害越高。";
    }
}
