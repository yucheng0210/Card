using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ThornsEffect : IEffect
{
    private string counterattackerLocation;
    private CharacterData counterattacker;
    private Dictionary<string, EnemyData> currentEnemyList;
    private Dictionary<string, int> currentOnceBattlePositiveList;
    private string typeName;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        currentEnemyList = BattleManager.Instance.CurrentEnemyList;
        typeName = GetType().Name;
        if (fromLocation == BattleManager.Instance.CurrentPlayerLocation)
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
        if (!currentOnceBattlePositiveList.ContainsKey(typeName))
        {
            EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, counterattacker, EventTakeDamage);
        }
        BattleManager.Instance.AddState(currentOnceBattlePositiveList, typeName, value);
        counterattackerLocation = fromLocation;
    }

    private void EventTakeDamage(params object[] args)
    {
        if (counterattackerLocation != (string)args[2])
        {
            return;
        }
        int damage = (int)args[1];
        int counterattackDamage = currentOnceBattlePositiveList[typeName];
        CharacterData counterattackTarget = (CharacterData)args[4];
        string targetLocation = currentEnemyList.FirstOrDefault(x => x.Value == counterattackTarget).Key ?? BattleManager.Instance.CurrentPlayerLocation;
        BattleManager.Instance.TakeDamage(counterattacker, counterattackTarget, counterattackDamage, targetLocation, 0.5f);
    }
    public string SetTitleText()
    {
        return "荊棘反傷";
    }

    public string SetDescriptionText()
    {
        return "受到傷害時反射傷害。";
    }
}
