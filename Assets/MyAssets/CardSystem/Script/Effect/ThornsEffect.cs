using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThornsEffect : IEffect
{
    private string counterattackerID;
    private CharacterData counterattacker;
    private int counterattackDamageMultiplier;
    private Dictionary<string, EnemyData> currentEnemyList;
    public void ApplyEffect(int value, string target)
    {
        Dictionary<string, int> currentOnceBattlePositiveList;
        currentEnemyList = BattleManager.Instance.CurrentEnemyList;
        if (target == BattleManager.Instance.CurrentLocationID)
        {
            counterattacker = BattleManager.Instance.CurrentPlayerData;
            currentOnceBattlePositiveList = BattleManager.Instance.CurrentOnceBattlePositiveList;
        }
        else if (currentEnemyList.TryGetValue(target, out var enemyData))
        {
            counterattacker = enemyData;
            currentOnceBattlePositiveList = enemyData.EnemyTrans.GetComponent<Enemy>().EnemyOnceBattlePositiveList;
        }
        else
        {
            Debug.LogWarning($"Target {target} not found in CurrentEnemyList.");
            return;
        }

        if (!currentOnceBattlePositiveList.TryGetValue(GetType().Name, out var existingValue))
        {
            existingValue = 0;
        }
        currentOnceBattlePositiveList[GetType().Name] = existingValue + value;

        counterattackDamageMultiplier = currentOnceBattlePositiveList[GetType().Name];
        counterattackerID = target;

        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
    }

    private void EventTakeDamage(params object[] args)
    {
        if (counterattackerID != (string)args[2])
            return;

        var damage = (int)args[1];
        var counterattackDamage = Mathf.RoundToInt(damage * (counterattackDamageMultiplier / 100f));
        
        var counterattackTarget = (CharacterData)args[4];
        var targetLocationID = currentEnemyList.FirstOrDefault(x => x.Value == counterattackTarget).Key ?? BattleManager.Instance.CurrentLocationID;

        BattleManager.Instance.TakeDamage(counterattacker, counterattackTarget, counterattackDamage, targetLocationID);

        if (!currentEnemyList.ContainsKey(counterattackerID) && BattleManager.Instance.CurrentLocationID != counterattackerID)
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
    }

    public Sprite SetIcon()
    {
        return Resources.Load<Sprite>("EffectImage/FightingSpirit");
    }
}
