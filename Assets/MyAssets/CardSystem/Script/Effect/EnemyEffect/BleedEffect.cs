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
        attacker = (EnemyData)BattleManager.Instance.IdentifyCharacter(fromLocation);
        bleedCount = value;
        typeName = GetType().Name;
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
        EventManager.Instance.AddEventRegister(EventDefinition.eventUseCard, EventUseCard);
    }

    private void EventTakeDamage(params object[] args)
    {
        if (!currentEnemyList.ContainsValue(attacker))
        {
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
            return;
        }
        CharacterData damageSource = (CharacterData)args[4];
        if (attacker == damageSource && BattleManager.Instance.MyBattleType == BattleManager.BattleType.Enemy)
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

    private void EventUseCard(params object[] args)
    {
        // Unregister the event if the defender is not at the current location
        if (!currentEnemyList.ContainsValue(attacker))
        {
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventUseCard, EventUseCard);
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
        return "攻擊時會受到等同於流血效果層數的傷害。";
    }
    public string SetPassiveEffectDescriptionText()
    {
        return "攻擊時對敵人施加流血效果。";
    }
}
