using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoisonedEffect : IEffect
{
    private EnemyData attacker;
    private int poisonCount;
    private Dictionary<string, EnemyData> currentEnemyList;
    private Dictionary<string, int> currentNegativeState;
    private string typeName;

    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        // Set attacker and poisonCount
        attacker = BattleManager.Instance.CurrentEnemyList[fromLocation];
        poisonCount = value;
        // Register events
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
        EventManager.Instance.AddEventRegister(EventDefinition.eventMove, EventMove);
        typeName = GetType().Name;

        // Remove this skill from attacker's passive skills
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
        CharacterData damageSource = (CharacterData)args[4];
        if (attacker == damageSource)
        {
            BattleManager.Instance.AddState(currentNegativeState, typeName, poisonCount);
        }
    }

    private void EventMove(params object[] args)
    {
        if (!currentEnemyList.ContainsValue(attacker))
        {
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventMove, EventMove);
            return;
        }
        BattleManager.Instance.TakeDamage(attacker, BattleManager.Instance.CurrentPlayerData, currentNegativeState[typeName], BattleManager.Instance.CurrentPlayerLocation, 0);
    }
    public string SetTitleText()
    {
        return "致命毒液";
    }
    public string SetPassiveEffectDescriptionText() { return "攻擊命中敵人附帶致命毒液。"; }
    public string SetDescriptionText()
    {
        return "移動時造成層數相等的傷害。";
    }
}
