using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonedEffect : IEffect
{
    private EnemyData attacker;
    private int poisonCount;
    private string defenderLocation;

    public void ApplyEffect(int value, string target)
    {
        attacker = BattleManager.Instance.CurrentEnemyList[target];
        poisonCount = value;
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
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
                BattleManager.Instance.CurrentNegativeState[GetType().Name] += poisonCount;
            else
                BattleManager.Instance.CurrentNegativeState.Add(GetType().Name, poisonCount);
        }
    }

    private void EventPlayerTurn(params object[] args)
    {
        if (BattleManager.Instance.CurrentLocationID != defenderLocation && !BattleManager.Instance.CurrentEnemyList.ContainsKey(defenderLocation))
        {
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
            return;
        }
        BattleManager.Instance.CurrentNegativeState[GetType().Name] *= 2;
    }

    public Sprite SetIcon()
    {
        throw new System.NotImplementedException();
    }
}
