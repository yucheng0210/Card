using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BleedEffect : IEffect
{
    private EnemyData attacker;
    private int bleedCount;
    private string defenderLocation;
    public void ApplyEffect(int value, string target)
    {
        attacker = BattleManager.Instance.CurrentEnemyList[target];
        bleedCount = value;
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
        EventManager.Instance.AddEventRegister(EventDefinition.eventMove, EventMove);
        attacker.PassiveSkills.Remove(GetType().Name);
    }
    private void EventTakeDamage(params object[] args)
    {
        defenderLocation = (string)args[2];
        if (BattleManager.Instance.CurrentLocationID != defenderLocation && !BattleManager.Instance.CurrentEnemyList.ContainsKey(defenderLocation))
        {
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
            return;
        }
        if (attacker == (CharacterData)args[4])
        {
            if (BattleManager.Instance.CurrentNegativeState.ContainsKey(GetType().Name))
                BattleManager.Instance.CurrentNegativeState[GetType().Name] += bleedCount;
            else
                BattleManager.Instance.CurrentNegativeState.Add(GetType().Name, bleedCount);
        }
    }
    private void EventMove(params object[] args)
    {
        if (BattleManager.Instance.CurrentLocationID != defenderLocation && !BattleManager.Instance.CurrentEnemyList.ContainsKey(defenderLocation))
        {
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventMove, EventMove);
            return;
        }
        BattleManager.Instance.TakeDamage(attacker, BattleManager.Instance.CurrentPlayerData,
        BattleManager.Instance.CurrentNegativeState[GetType().Name], BattleManager.Instance.CurrentLocationID);
    }
    public Sprite SetIcon()
    {
        throw new System.NotImplementedException();
    }
}
