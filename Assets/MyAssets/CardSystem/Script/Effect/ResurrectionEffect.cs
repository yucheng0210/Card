using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResurrectionEffect : IEffect
{
    private int reciprocalCount = 1;
    private int recoverCount;
    private string id;
    private EnemyData enemyData;
    public void ApplyEffect(int value, string target)
    {
        enemyData = BattleManager.Instance.CurrentEnemyList[target];
        recoverCount = 1;
        id = target;
        EventManager.Instance.AddEventRegister(EventDefinition.eventEnemyTurn, EventEnemyTurn);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }
    private void EventEnemyTurn(params object[] args)
    {
        reciprocalCount--;
        if (reciprocalCount <= 0)
        {
            Enemy enemy = enemyData.EnemyTrans.GetComponent<Enemy>();
            BattleManager.Instance.Recover(enemyData, recoverCount, id);
            enemy.MyAnimator.SetTrigger("isResurrection");
            EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventEnemyTurn, EventEnemyTurn);
        }
    }
    public Sprite SetIcon()
    {
        throw new System.NotImplementedException();
    }
}