using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuriousEffect : IEffect
{
    private string targetLocation;
    private int attackIncreaseCount;
    private EnemyData enemyData;
    public void ApplyEffect(int value, string target)
    {
        enemyData = BattleManager.Instance.CurrentEnemyList[target];
        targetLocation = target;
        attackIncreaseCount = enemyData.CurrentAttack * value / 100;
        enemyData.PassiveSkills.Remove(GetType().Name);
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
    }
    private void EventTakeDamage(params object[] args)
    {
        CharacterData attacker = (CharacterData)args[4];
        if (!BattleManager.Instance.CurrentEnemyList.ContainsKey(targetLocation))
        {
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
            return;
        }
        if (BattleManager.Instance.CurrentEnemyList[targetLocation] == attacker)
            enemyData.CurrentAttack += attackIncreaseCount;
    }
    public Sprite SetIcon()
    {
        throw new System.NotImplementedException();
    }

}
