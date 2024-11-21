using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ThornsEffect : IEffect
{
    private string counterattackerID;
    private CharacterData counterattacker;
    private Dictionary<string, EnemyData> currentEnemyList;
    private Dictionary<string, int> currentOnceBattlePositiveList;
    private string typeName;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        currentEnemyList = BattleManager.Instance.CurrentEnemyList;
        typeName = GetType().Name;
        if (fromLocation == BattleManager.Instance.CurrentLocationID)
        {
            counterattacker = BattleManager.Instance.CurrentPlayerData;
            currentOnceBattlePositiveList = BattleManager.Instance.CurrentOnceBattlePositiveList;
        }
        else if (currentEnemyList.TryGetValue(fromLocation, out var enemyData))
        {
            counterattacker = enemyData;
            currentOnceBattlePositiveList = enemyData.EnemyTrans.GetComponent<Enemy>().EnemyOnceBattlePositiveList;
        }
        else
        {
            Debug.LogWarning($"Target {fromLocation} not found in CurrentEnemyList.");
            return;
        }

        if (!currentOnceBattlePositiveList.TryGetValue(typeName, out var existingValue))
        {
            existingValue = 0;
        }
        currentOnceBattlePositiveList[typeName] = existingValue + value;
        counterattackerID = fromLocation;
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
    }

    private void EventTakeDamage(params object[] args)
    {
        if (counterattackerID != (string)args[2] || args.Length < 5)
        {
            return;
        }
        var damage = (int)args[1];
        var counterattackDamage = Mathf.RoundToInt(damage * (currentOnceBattlePositiveList[typeName] / 100f));
        var counterattackTarget = (CharacterData)args[4];
        var targetLocationID = currentEnemyList.FirstOrDefault(x => x.Value == counterattackTarget).Key ?? BattleManager.Instance.CurrentLocationID;
        BattleManager.Instance.TakeDamage(counterattacker, counterattackTarget, counterattackDamage, targetLocationID, 0.5f);
        if (!currentEnemyList.ContainsKey(counterattackerID) && BattleManager.Instance.CurrentLocationID != counterattackerID)
        {
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
        }
    }
    public string SetTitleText()
    {
        return "荊棘反傷";
    }

    public string SetDescriptionText()
    {
        return "受到攻擊時，將部分比例的傷害反彈回去。";
    }
}
