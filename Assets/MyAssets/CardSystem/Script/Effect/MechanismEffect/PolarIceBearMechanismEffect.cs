using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class PolarIceBearMechanismEffect : IEffect
{
    private EnemyData meleeEnemyData;
    private EnemyData longDistanceEnemyData;
    private Enemy meleeEnemy;
    private Enemy longDistanceEnemy;
    private string meleeEnemyLocation;
    private string longDistanceEnemyLocation;
    private bool isInExchangeRange;
    private int exchangeCount = 1;
    private Dictionary<string, EnemyData> currentEnemyList;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        currentEnemyList = BattleManager.Instance.CurrentEnemyList;
        meleeEnemyLocation = currentEnemyList.ElementAt(0).Key;
        longDistanceEnemyLocation = currentEnemyList.ElementAt(1).Key;
        meleeEnemyData = currentEnemyList[meleeEnemyLocation];
        longDistanceEnemyData = currentEnemyList[longDistanceEnemyLocation];
        meleeEnemy = meleeEnemyData.EnemyTrans.GetComponent<Enemy>();
        longDistanceEnemy = longDistanceEnemyData.EnemyTrans.GetComponent<Enemy>();
        longDistanceEnemy.IsSuspendedAnimation = true;
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, meleeEnemyData, EventPlayerTurn);
        EventManager.Instance.AddEventRegister(EventDefinition.eventMove, meleeEnemyData, EventMove);
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, meleeEnemyData, EventTakeDamage);
        meleeEnemyData.MaxPassiveSkillsList.Remove(GetType().Name);
    }
    private void EventPlayerTurn(params object[] args)
    {
        meleeEnemyLocation = BattleManager.Instance.GetEnemyKey(meleeEnemyData);
        longDistanceEnemyLocation = BattleManager.Instance.GetEnemyKey(longDistanceEnemyData);
        exchangeCount = 1;
    }
    private void EventMove(params object[] args)
    {
        List<string> surroundingsLocationList = BattleManager.Instance.GetActionRangeTypeList(longDistanceEnemyLocation, 1,
        BattleManager.CheckEmptyType.EnemyAttack, BattleManager.ActionRangeType.Surrounding);
        isInExchangeRange = surroundingsLocationList.Contains(BattleManager.Instance.CurrentPlayerLocation);
    }
    private void EventTakeDamage(params object[] args)
    {
        if (meleeEnemyData.CurrentHealth <= 0)
        {
            longDistanceEnemy.IsSuspendedAnimation = false;
        }
        if (args[5] == longDistanceEnemyData)
        {
            int healthCount = Mathf.Min(BattleManager.Instance.GetPercentage(longDistanceEnemyData.MaxHealth, 30), meleeEnemyData.CurrentHealth - 1);
            if (!longDistanceEnemy.IsSuspendedAnimation)
            {
                return;
            }
            if (longDistanceEnemyData.CurrentHealth <= 0)
            {
                string effectName = "HealthBalanceEffect=" + healthCount.ToString();
                BattleManager.Instance.TemporaryChangeEffect(meleeEnemy, effectName, longDistanceEnemyLocation);
                longDistanceEnemy.MyActionType = Enemy.ActionType.None;
                longDistanceEnemy.InfoTitle.text = "?";
                longDistanceEnemy.InfoDescription.text = "???";
                longDistanceEnemy.IsSuspendedAnimation = false;
            }
            else if (isInExchangeRange && exchangeCount > 0)
            {
                Transform meleeEnemyTrans = meleeEnemyData.EnemyTrans;
                Transform longEnemyTrans = longDistanceEnemyData.EnemyTrans;
                meleeEnemy.EnemyImage.material = meleeEnemy.DissolveMaterial;
                TweenCallback tweenCallback = () =>
                {
                    TweenCallback endTweenCallback = () => { };
                    BattleManager.Instance.SetDissolveMaterial(meleeEnemy.DissolveMaterial, 0.0f, 1, endTweenCallback);
                    BattleManager.Instance.ExchangePos(meleeEnemyTrans, currentEnemyList, meleeEnemyLocation, longEnemyTrans, longDistanceEnemyLocation, currentEnemyList);
                    meleeEnemyLocation = BattleManager.Instance.GetEnemyKey(meleeEnemyData);
                    longDistanceEnemyLocation = BattleManager.Instance.GetEnemyKey(longDistanceEnemyData);
                    AudioManager.Instance.SEAudio(5);
                };
                BattleManager.Instance.SetDissolveMaterial(meleeEnemy.DissolveMaterial, 1.0f, 0, tweenCallback);
                longDistanceEnemy.EnemyImage.material = longDistanceEnemy.DissolveMaterial;
                TweenCallback tweenCallback_2 = () =>
                {
                    TweenCallback endTweenCallback = () => { };
                    BattleManager.Instance.SetDissolveMaterial(longDistanceEnemy.DissolveMaterial, 0.0f, 1, endTweenCallback);
                    AudioManager.Instance.SEAudio(5);
                };
                BattleManager.Instance.SetDissolveMaterial(longDistanceEnemy.DissolveMaterial, 1.0f, 0, tweenCallback_2);
                meleeEnemy.RefreshAttackIntent();
                longDistanceEnemy.RefreshAttackIntent();
                exchangeCount--;
            }
        }
    }
    public string SetTitleText()
    {
        return "極冬領域";
    }

    public string SetDescriptionText()
    {
        return "霜塔神使受擊時且玩家在附近，冰河君主會與其交換位置；霜塔神使陣亡時，冰河君主可發動生命平衡。";
    }
}
