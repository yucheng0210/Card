using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class ParasiticEffect : IEffect
{
    private EnemyData parasite;
    private PlayerData host;
    private int damage;
    private int sporeCount;

    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        if (!BattleManager.Instance.CurrentEnemyList.TryGetValue(fromLocation, out parasite))
        {
            Debug.LogWarning($"Target {fromLocation} not found in CurrentEnemyList.");
            return;
        }
        host = BattleManager.Instance.CurrentPlayerData;
        damage = value;
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
        EventManager.Instance.AddEventRegister(EventDefinition.eventEnemyTurn, EventEnemyTurn);
    }

    private void EventTakeDamage(params object[] args)
    {
        if ((CharacterData)args[4] == parasite)
        {
            if (!BattleManager.Instance.CurrentEnemyList.ContainsValue(parasite))
            {
                EventManager.Instance.RemoveEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
                return;
            }
            BattleManager.Instance.AddCard(5002);
        }
    }

    private void EventEnemyTurn(params object[] args)
    {
        if (!BattleManager.Instance.CurrentEnemyList.ContainsValue(parasite))
        {
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventEnemyTurn, EventEnemyTurn);
            return;
        }
        sporeCount = BattleManager.Instance.GetCardCount(5002);
        if (sporeCount == 0)
        {
            return;
        }
        int currentDamage = damage * sporeCount;
        string enemyLocation = BattleManager.Instance.GetEnemyKey(parasite);
        CharacterData parasiteRepresent = new EnemyData();
        BattleManager.Instance.Recover(parasite, currentDamage, enemyLocation);
        BattleManager.Instance.TakeDamage(parasiteRepresent, host, currentDamage, BattleManager.Instance.CurrentPlayerLocation, 0.5f);
    }
    public bool IsShowEffectCount() { return false; }
    public string SetTitleText()
    {
        return "孢子寄生";
    }

    public string SetDescriptionText()
    {
        return "每次攻擊命中時添加寄生詛咒卡片至牌組。";
    }
}