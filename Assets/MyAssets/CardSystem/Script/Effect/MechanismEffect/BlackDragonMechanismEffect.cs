using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackDragonMechanismEffect : IEffect
{
    private int maxDamageCount = 250;
    private string enemyLocation;
    private EnemyData enemyData;
    private Enemy enemy;
    private int stageCount = 1;
    private int thirdStageCount = 0;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        enemyData = BattleManager.Instance.CurrentEnemyList[fromLocation];
        enemy = enemyData.EnemyTrans.GetComponent<Enemy>();
        enemyLocation = fromLocation;
        enemyData.DamageLimit = maxDamageCount;
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, enemyData, EventTakeDamage);
        enemyData.MaxPassiveSkillsList.Remove(GetType().Name);
    }
    private void ThirdStageEnemyTurn(params object[] args)
    {
        if (enemyData.CurrentShield > 0)
        {
            switch (thirdStageCount)
            {
                case 1:
                    enemy.AdditionPower++;
                    BattleManager.Instance.ShowCharacterStatusClue(enemy.StatusClueTrans, "力量提升", 0);
                    break;
                case 2:
                    enemy.AdditionAttackCount = 5;
                    BattleManager.Instance.ShowCharacterStatusClue(enemy.StatusClueTrans, "攻擊數提升", 0);
                    break;
            }
        }
        enemyData.CurrentShield = 0;
        if (thirdStageCount == 3)
        {
            EventManager.Instance.AddEventRegister(EventDefinition.eventAfterEnemyAttack, FinishThirdStage);
            return;
        }
        thirdStageCount++;
    }
    private void ThirdStageStagePlayerTurn(params object[] args)
    {
        Debug.Log(thirdStageCount);
        int shieldCount = 75 * (thirdStageCount + 1);
        BattleManager.Instance.TemporaryChangeShield(enemy, shieldCount);
        if (thirdStageCount == 3)
        {
            BattleManager.CheckEmptyType checkEmptyType = BattleManager.CheckEmptyType.EnemyAttack;
            BattleManager.ActionRangeType actionRangeType = BattleManager.ActionRangeType.AllZone;
            List<string> emptyPlaceList = BattleManager.Instance.GetActionRangeTypeList("", 0, checkEmptyType, actionRangeType);
            EffectFactory.Instance.CreateEffect("RechargeEffect").ApplyEffect(2, enemyLocation, BattleManager.Instance.CurrentPlayerLocation);
            BattleManager.Instance.ShowCharacterStatusClue(enemy.StatusClueTrans, "充能", 0);
            BattleManager.Instance.TemporaryChangeAttack(enemy, BattleManager.Instance.CurrentPlayerLocation, emptyPlaceList, 10);
        }
    }
    private void FinishThirdStage(params object[] args)
    {
        enemyData.DamageLimit = enemyData.CurrentHealth;
        enemy.AdditionPower--;
        enemy.AdditionAttackCount = 0;
        enemy.IsSpecialAction = true;
        enemyData.CurrentAttackOrderIndex = 0;
        EventManager.Instance.RemoveEventRegister(EventDefinition.eventEnemyTurn, ThirdStageEnemyTurn);
        EventManager.Instance.RemoveEventRegister(EventDefinition.eventPlayerTurn, ThirdStageStagePlayerTurn);
        EventManager.Instance.RemoveEventRegister(EventDefinition.eventAfterEnemyAttack, FinishThirdStage);
    }
    private void EventTakeDamage(params object[] args)
    {
        if (args[5] == enemyData)
        {
            enemyData.DamageLimit -= (int)args[6];
            switch (enemyData.CurrentHealth)
            {
                case 749:
                    if (stageCount == 1)
                    {
                        stageCount++;
                        BattleManager.Instance.TemporaryChangeEffect(enemy, "FlyingEffect=50", BattleManager.Instance.CurrentPlayerLocation);
                        EffectFactory.Instance.CreateEffect("DizzinessEffect").ApplyEffect(0, "", "");
                        BattleManager.Instance.ShowCharacterStatusClue(enemy.StatusClueTrans, "怒吼", 0);
                        enemyData.DamageLimit = maxDamageCount;
                    }
                    break;
                case 499:
                    if (stageCount == 2)
                    {
                        stageCount++;
                        ThirdStageStagePlayerTurn();
                        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, ThirdStageStagePlayerTurn);
                        EventManager.Instance.AddEventRegister(EventDefinition.eventEnemyTurn, ThirdStageEnemyTurn);
                        BattleManager.Instance.ShowCharacterStatusClue(enemy.StatusClueTrans, "怒吼", 0);
                        EffectFactory.Instance.CreateEffect("DizzinessEffect").ApplyEffect(0, "", "");
                    }
                    if (thirdStageCount == 3 && enemyData.CurrentShield == 0)
                    {
                        enemy.AdditionAttackMultiplier = 1;
                        BattleManager.Instance.SetEnemyAttackPower(enemy, enemyData);
                    }
                    break;
            }
        }
    }
    public string SetTitleText()
    {
        return "熔岩領域";
    }
    public string SetDescriptionText()
    {
        return "黑龍有三階段，第二階段會發動飛行，第三階段要破除三次護盾減少全範圍攻擊的傷害，攻擊完會發動終焉噬焰。";
    }

}
