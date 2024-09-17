using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonedEffect : IEffect
{
    private EnemyData attacker;
    private int poisonCount;
    private Dictionary<string, EnemyData> currentEnemyList;
    private Dictionary<string, int> currentNegativeState;
    private string typeName;

    public void ApplyEffect(int value, string target)
    {
        // Set attacker and poisonCount
        attacker = BattleManager.Instance.CurrentEnemyList[target];
        poisonCount = value;
        // Register events
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
        typeName = GetType().Name;

        // Remove this skill from attacker's passive skills
        attacker.PassiveSkills.Remove(typeName);
        currentEnemyList = BattleManager.Instance.CurrentEnemyList;
        currentNegativeState = BattleManager.Instance.CurrentNegativeState;
    }

    private void EventTakeDamage(params object[] args)
    {
        // If the defender is no longer valid, unregister event and exit
        if (!currentEnemyList.ContainsValue(attacker))
        {
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
            return;
        }

        // Apply poison effect if the attacker matches
        CharacterData damageSource = (CharacterData)args[4];
        if (attacker == damageSource)
        {
            if (currentNegativeState.ContainsKey(typeName))
                currentNegativeState[typeName] += poisonCount;
            else
                currentNegativeState.Add(typeName, poisonCount);
        }
    }

    private void EventPlayerTurn(params object[] args)
    {

        // If the defender is no longer valid, unregister event and exit
        if (!currentEnemyList.ContainsValue(attacker))
        {
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
            return;
        }
        if (!currentNegativeState.ContainsKey(typeName))
            return;
        // Double the poison effect on player's turn
        if (currentNegativeState.ContainsKey(typeName))
            currentNegativeState[typeName] *= 2;
    }

    public Sprite SetIcon()
    {
        return Resources.Load<Sprite>("EffectImage/CantMove");
    }
}
