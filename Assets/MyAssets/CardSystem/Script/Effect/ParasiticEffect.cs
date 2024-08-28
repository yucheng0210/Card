using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ParasiticEffect : IEffect
{
    private EnemyData parasite;
    private PlayerData host;
    private string recoverTargetID;
    private int damage;
    public void ApplyEffect(int value, string target)
    {
        parasite = BattleManager.Instance.CurrentEnemyList[target];
        recoverTargetID = target;
        host = BattleManager.Instance.CurrentPlayerData;
        parasite.PassiveSkills.Remove(GetType().Name);
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
    }
    private void EventTakeDamage(params object[] args)
    {
        if (!BattleManager.Instance.CurrentEnemyList.ContainsValue(parasite))
        {
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
            return;
        }
        BattleManager.Instance.AddCard(5002);
        damage = host.MaxHealth / 100 * 2;
        EventManager.Instance.AddEventRegister(EventDefinition.eventEnemyTurn, EventEnemyTurn);
    }
    private void EventEnemyTurn(params object[] args)
    {
        if (!BattleManager.Instance.CurrentEnemyList.ContainsValue(parasite))
        {
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventEnemyTurn, EventEnemyTurn);
            return;
        }
        BattleManager.Instance.Recover(parasite, damage, recoverTargetID);
        BattleManager.Instance.TakeDamage(parasite, host, damage, BattleManager.Instance.CurrentLocationID);
    }
    public Sprite SetIcon()
    {
        throw new System.NotImplementedException();
    }
}
