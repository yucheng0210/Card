using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BleedEffect : IEffect
{
    private EnemyData attacker;
    private int bleedCount;
    Dictionary<string, EnemyData> currentEnemyList = new();
    Dictionary<string, int> currentNegativeState = new();
    private string typeName;
    public void ApplyEffect(int value, string target)
    {
        currentEnemyList = BattleManager.Instance.CurrentEnemyList;
        currentNegativeState = BattleManager.Instance.CurrentNegativeState;
        // Set attacker and bleedCount
        attacker = currentEnemyList.ContainsKey(target) ? currentEnemyList[target] : BattleManager.Instance.CurrentMinionsList[target];
        bleedCount = value;
        typeName = GetType().Name;
        // Register events
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
        EventManager.Instance.AddEventRegister(EventDefinition.eventMove, EventMove);

        // Remove this effect from attacker's passive skills
        attacker.PassiveSkills.Remove(typeName);

    }

    private void EventTakeDamage(params object[] args)
    {
        // Unregister the event if the defender is not at the current location
        if (!currentEnemyList.ContainsValue(attacker))
        {
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
            return;
        }
        // Apply bleed effect if the attacker matches the damage source
        CharacterData damageSource = (CharacterData)args[4];
        if (attacker == damageSource)
        {
            if (currentNegativeState.ContainsKey(typeName))
                currentNegativeState[typeName] += bleedCount;
            else
                currentNegativeState.Add(typeName, bleedCount);
        }
    }

    private void EventMove(params object[] args)
    {

        // Unregister the event if the defender is not at the current location
        if (!currentEnemyList.ContainsValue(attacker))
        {
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventMove, EventMove);
            return;
        }
        if (!currentNegativeState.ContainsKey(typeName))
            return;
        // Apply damage to the player
        int bleedDamage = currentNegativeState[typeName];
        BattleManager.Instance.TakeDamage(attacker, BattleManager.Instance.CurrentPlayerData, bleedDamage, BattleManager.Instance.CurrentLocationID);
    }

    public Sprite SetIcon()
    {
        return Resources.Load<Sprite>("EffectImage/CantMove");
    }
}
