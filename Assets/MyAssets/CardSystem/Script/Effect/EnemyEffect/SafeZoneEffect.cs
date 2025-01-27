using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeZoneEffect : IEffect
{
    private EnemyData enemyData;
    private Enemy enemy;
    private List<string> safeZoneList = new List<string>();
    private List<string> attackList = new List<string>();
    private int safeZoneCount = 6;
    private int reciprocalCount = 3;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        enemyData = BattleManager.Instance.CurrentEnemyList[fromLocation];
        enemy = enemyData.EnemyTrans.GetComponent<Enemy>();
        RefreshSafeZone();
        reciprocalCount--;
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
    }
    private void RefreshSafeZone()
    {
        BattleManager.CheckEmptyType checkEmptyType = BattleManager.CheckEmptyType.EnemyAttack;
        BattleManager.ActionRangeType actionRangeType = BattleManager.ActionRangeType.AllZone;
        attackList = BattleManager.Instance.GetActionRangeTypeList("", 0, checkEmptyType, actionRangeType);
        for (int i = 0; i < attackList.Count; i++)
        {
            CheckerboardSlot checkerboardSlot = BattleManager.Instance.GetCheckerboardTrans(attackList[i]).GetComponent<CheckerboardSlot>();
            checkerboardSlot.SafeZoneImage.gameObject.SetActive(false);
        }
        for (int i = 0; i < safeZoneCount; i++)
        {
            int randomIndex = Random.Range(0, attackList.Count);
            CheckerboardSlot checkerboardSlot = BattleManager.Instance.GetCheckerboardTrans(attackList[randomIndex]).GetComponent<CheckerboardSlot>();
            checkerboardSlot.SafeZoneImage.gameObject.SetActive(true);
            safeZoneList.Add(attackList[randomIndex]);
            attackList.RemoveAt(randomIndex);
        }
    }

    private void EventPlayerTurn(params object[] args)
    {
        /*if (enemy.SpecialActionStage == 1)
        {
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
            safeZoneCount = 0;
            RefreshSafeZone();
            return;
        }*/
        if (reciprocalCount == 3)
        {
            RefreshSafeZone();
        }
        reciprocalCount--;
        if (reciprocalCount > 0)
        {
            return;
        }
        reciprocalCount = 3;
        BattleManager.Instance.TemporaryChangeAttack(enemy, BattleManager.Instance.CurrentPlayerLocation, attackList, 10);
        if (safeZoneCount >= 0)
        {
            safeZoneCount--;
        }
    }
    public string SetTitleText()
    {
        return "火焰";
    }
    public string SetDescriptionText()
    {
        return "";
    }
}
