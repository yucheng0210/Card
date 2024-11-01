using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BleedEffect : IEffect
{
    private EnemyData attacker;
    private int bleedCount;
    Dictionary<string, EnemyData> currentEnemyList = new();
    Dictionary<string, int> currentNegativeState = new();
    private string typeName;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        currentEnemyList = BattleManager.Instance.CurrentEnemyList;
        currentNegativeState = BattleManager.Instance.CurrentNegativeState;
        // Set attacker and bleedCount
        attacker = currentEnemyList.ContainsKey(fromLocation) ? currentEnemyList[fromLocation] : BattleManager.Instance.CurrentMinionsList[fromLocation];
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
            {
                currentNegativeState[typeName] += bleedCount;
            }
            else
            {
                currentNegativeState.Add(typeName, bleedCount);
            }
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
        {
            return;
        }
        // Apply damage to the player
        int bleedDamage = currentNegativeState[typeName];
        BattleManager.Instance.TakeDamage(attacker, BattleManager.Instance.CurrentPlayerData, bleedDamage, BattleManager.Instance.CurrentLocationID, 0.25f);
    }

    public string SetTitleText()
    {
        return "流血";
    }

    public string SetDescriptionText()
    {
        return "移動時會受到等同於流血效果層數的傷害。";
    }
    public string SetPassiveEffectDescriptionText()
    {
        return "攻擊時對敵人施加流血效果。";
    }
}
