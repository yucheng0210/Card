using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThornsEffect : IEffect
{
    private string counterattackerID;
    private string counterattackTargetID;
    private CharacterData counterattacker;
    private int counterattackDamage;
    private int counterattackDamageMultiplier;
    public void ApplyEffect(int value, string target)
    {
        Dictionary<string, int> currentOnceBattlePositiveList;
        if (target == BattleManager.Instance.CurrentLocationID)
        {
            currentOnceBattlePositiveList = BattleManager.Instance.CurrentOnceBattlePositiveList;
            counterattacker = BattleManager.Instance.CurrentPlayerData;
        }
        else
        {
            Enemy enemy = BattleManager.Instance.CurrentEnemyList[target].EnemyTrans.GetComponent<Enemy>();
            currentOnceBattlePositiveList = enemy.EnemyOnceBattlePositiveList;
            counterattacker = BattleManager.Instance.CurrentEnemyList[target];
        }
        if (currentOnceBattlePositiveList.ContainsKey(GetType().Name))
            currentOnceBattlePositiveList[GetType().Name] += 5;
        else
            currentOnceBattlePositiveList.Add(GetType().Name, 5);
        counterattackDamageMultiplier = currentOnceBattlePositiveList[GetType().Name];
        counterattackerID = target;
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
    }
    private void EventTakeDamage(params object[] args)
    {
        int damage = (int)args[1];
        string locationID = (string)args[2];
        counterattackDamage = damage / 100 * counterattackDamageMultiplier;
        CharacterData counterattackTarget = (CharacterData)args[4];
        counterattackTargetID = BattleManager.Instance.CurrentEnemyList.FirstOrDefault(x => x.Value == counterattackTarget).Key;
        if (counterattackTargetID == null)
            counterattackTargetID = BattleManager.Instance.CurrentLocationID;
        if (counterattackerID == locationID)
        {
            if (BattleManager.Instance.CurrentLocationID != locationID && !BattleManager.Instance.CurrentEnemyList.ContainsKey(locationID))
            {
                EventManager.Instance.RemoveEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
                return;
            }
            BattleManager.Instance.TakeDamage(counterattacker, counterattackTarget, counterattackDamage, counterattackTargetID);
        }

    }
    public Sprite SetIcon()
    {
        return Resources.Load<Sprite>("EffectImage/FightingSpirit");
    }
}
