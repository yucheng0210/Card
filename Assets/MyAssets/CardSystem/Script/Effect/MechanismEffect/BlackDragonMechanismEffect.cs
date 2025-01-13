using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class BlackDragonMechanismEffect : IEffect
{
    private int maxDamageCount = 250;
    private string enemyLocation;
    private EnemyData enemyData;
    private Enemy enemy;
    private int stage_2Count = 0;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        enemyData = BattleManager.Instance.CurrentEnemyList[fromLocation];
        enemy = enemyData.EnemyTrans.GetComponent<Enemy>();
        enemyLocation = fromLocation;
        enemyData.DamageLimit = maxDamageCount;
        enemyData.MaxHealth = 999;
        enemyData.CurrentHealth = enemyData.MaxHealth;
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, enemyData, EventTakeDamage);
    }
    private void SecondStageEnemyTurn(params object[] args)
    {
        if (stage_2Count == 3)
        {
            enemyData.DamageLimit = maxDamageCount;
            EventManager.Instance.AddEventRegister(EventDefinition.eventDrawCard, FinishSpecialActionStage_2);
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventEnemyTurn, SecondStageEnemyTurn);
            return;
        }
        if (enemyData.CurrentShield > 0)
        {
            enemyData.CurrentShield = 0;
            switch (stage_2Count)
            {
                case 1:
                    enemy.AdditionPower++;
                    BattleManager.Instance.ShowCharacterStatusClue(enemy.StatusClueTrans, "力量提升", 0);
                    break;
                case 2:
                    enemy.AdditionAttackCount = 5;
                    BattleManager.Instance.ShowCharacterStatusClue(enemy.StatusClueTrans, "攻擊次數提升", 0);
                    break;
            }
        }
        stage_2Count++;
        Debug.Log(stage_2Count);
    }
    private void SecondStageDrawCard(params object[] args)
    {
        Debug.Log(stage_2Count);
        int shieldCount = 75 * (stage_2Count + 1);
        BattleManager.Instance.TemporaryChangeShield(enemy, shieldCount);
        if (stage_2Count == 2)
        {
            BattleManager.CheckEmptyType checkEmptyType = BattleManager.CheckEmptyType.EnemyAttack;
            BattleManager.ActionRangeType actionRangeType = BattleManager.ActionRangeType.AllZone;
            List<string> emptyPlaceList = BattleManager.Instance.GetActionRangeTypeList("", 0, checkEmptyType, actionRangeType);
            BattleManager.Instance.GetShield(enemyData, shieldCount);
            EffectFactory.Instance.CreateEffect("RechargeEffect").ApplyEffect(2, enemyLocation, BattleManager.Instance.CurrentLocationID);
            BattleManager.Instance.TemporaryChangeAttack(enemy, BattleManager.Instance.CurrentLocationID, emptyPlaceList, 10);
        }
    }
    private void FinishSpecialActionStage_2(params object[] args)
    {
        enemy.AdditionPower--;
        enemy.AdditionAttackCount = 0;
        EventManager.Instance.RemoveEventRegister(EventDefinition.eventPlayerTurn, FinishSpecialActionStage_2);
    }
    private void EventTakeDamage(params object[] args)
    {
        if (args[5] == enemyData)
        {
            enemyData.DamageLimit -= (int)args[6];
            switch (enemyData.CurrentHealth)
            {
                case 749:
                    SecondStageDrawCard();
                    EventManager.Instance.AddEventRegister(EventDefinition.eventDrawCard, SecondStageDrawCard);
                    EventManager.Instance.AddEventRegister(EventDefinition.eventEnemyTurn, SecondStageEnemyTurn);
                    BattleManager.Instance.ShowCharacterStatusClue(enemy.StatusClueTrans, "怒吼", 0);
                    Debug.Log(enemyData.DamageLimit);
                    //EffectFactory.Instance.CreateEffect("DizzinessEffect").ApplyEffect(0, "", "");
                    break;
                case 499:
                    if (stage_2Count == 2 && enemyData.CurrentShield == 0)
                    {
                        enemy.AdditionAttackMultiplier = 1;
                    }
                    break;
            }
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
