using System.Collections;
using System.Collections.Generic;
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
        damage = Mathf.RoundToInt(value * (host.MaxHealth / 100f));
        parasite.PassiveSkills.Remove(GetType().Name);
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
        // Register enemy turn event
        EventManager.Instance.AddEventRegister(EventDefinition.eventEnemyTurn, EventEnemyTurn);
    }

    private void EventTakeDamage(params object[] args)
    {
        if (args.Length >= 5 && (CharacterData)args[4] == parasite)
        {
            if (!BattleManager.Instance.CurrentEnemyList.ContainsValue(parasite))
            {
                EventManager.Instance.RemoveEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
                return;
            }

            // Trigger card addition
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
            return;
        int currentDamage = damage * sporeCount;
        string enemyLocation = BattleManager.Instance.GetEnemyKey(parasite);
        CharacterData parasiteRepresent = new EnemyData();
        // Recover parasite and damage host
        BattleManager.Instance.Recover(parasite, currentDamage, enemyLocation);
        BattleManager.Instance.TakeDamage(parasiteRepresent, host, currentDamage, BattleManager.Instance.CurrentLocationID, 0.5f);
    }

    public string SetTitleText()
    {
        return "孢子寄生";
    }

    public string SetDescriptionText()
    {
        return "給予寄生卡片。";
    }
}