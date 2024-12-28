using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SacrificeEffect : IEffect
{
    private EnemyData enemyData;
    private Enemy enemy;
    private EnemyData masterEnemyData;
    private string typeName;
    private int triggerHealth;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        if (!BattleManager.Instance.CurrentMinionsList.TryGetValue(fromLocation, out enemyData))
        {
            return;
        }
        triggerHealth = value;
        typeName = GetType().Name;
        enemy = enemyData.EnemyTrans.GetComponent<Enemy>();
        masterEnemyData = BattleManager.Instance.CurrentEnemyList[enemy.MasterLocation];
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, enemyData, EventTakeDamage);
        EventManager.Instance.AddEventRegister(EventDefinition.eventEnemyTurn, enemyData, EventEnemyTurn);
    }
    private void EventTakeDamage(params object[] args)
    {
        if (enemyData.CurrentHealth > enemyData.MaxHealth * (triggerHealth / 100f))
        {
            return;
        }
        enemy.MyActionType = Enemy.ActionType.None;
        Sprite effectSprite = ((IEffect)this).SetIcon();
        if (effectSprite == null)
        {
            effectSprite = EffectFactory.Instance.CreateEffect("KnockBackEffect").SetIcon();
        }
        enemy.EnemyEffectImage.GetComponent<Image>().sprite = effectSprite;
        enemy.EnemyEffectImage.SetActive(true);
        enemy.InfoTitle.text = SetTitleText();
        enemy.InfoDescription.text = SetDescriptionText();
        enemy.EnemyOnceBattlePositiveList.Add(typeName, 1);
        enemy.SetInfoGroupEventTrigger();
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
        EventManager.Instance.RemoveEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
    }
    private void EventEnemyTurn(params object[] args)
    {
        if (enemyData.CurrentHealth > enemyData.MaxHealth * (triggerHealth / 100f))
        {
            return;
        }
        enemy.EnemyOnceBattlePositiveList.Remove(typeName);
        enemy.IsDeath = false;
        string location = BattleManager.Instance.GetEnemyKey(enemyData);
        string masterLocation = BattleManager.Instance.GetEnemyKey(masterEnemyData);
        BattleManager.Instance.Recover(masterEnemyData, enemyData.CurrentHealth, masterLocation);
        BattleManager.Instance.TakeDamage(enemyData, enemyData, enemyData.CurrentHealth, location, 0.5f);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
        EventManager.Instance.RemoveEventRegister(EventDefinition.eventEnemyTurn, EventEnemyTurn);
    }
    public string SetTitleText()
    {
        return "獻祭";
    }
    public string SetDescriptionText()
    {
        return "當血量過低時，會將自身剩餘的血量值回復給本體。";
    }

}
