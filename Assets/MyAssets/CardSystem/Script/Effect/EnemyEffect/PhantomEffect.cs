using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
public class PhantomEffect : IEffect
{
    private EnemyData enemyData;
    private int attackCount = 2;
    private string enemyLocation;
    private Dictionary<string, EnemyData> currentMinionsList;
    private Enemy enemy;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        currentMinionsList = BattleManager.Instance.CurrentMinionsList;
        enemyData = (EnemyData)BattleManager.Instance.IdentifyCharacter(fromLocation);
        enemy = enemyData.EnemyTrans.GetComponent<Enemy>();
        enemyLocation = fromLocation;
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, enemyData, EventTakeDamage);
        if (enemyData.IsMinion)
        {
            return;
        }
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, enemyData, EventPlayerTurn);
    }
    private void EventPlayerTurn(params object[] args)
    {
        if (currentMinionsList.Count == 0)
        {
            return;
        }
        for (int i = 0; i < currentMinionsList.Count; i++)
        {
            string key = currentMinionsList.ElementAt(i).Key;
            currentMinionsList[key].CurrentHealth = enemyData.CurrentHealth;
        }
        attackCount = 2;
        int randomIndex = Random.Range(0, currentMinionsList.Count);
        string minionLocation = currentMinionsList.ElementAt(randomIndex).Key;
        BattleManager.Instance.ExchangePos(enemyData.EnemyTrans, BattleManager.Instance.CurrentEnemyList, enemyLocation,
        currentMinionsList[minionLocation].EnemyTrans, minionLocation, currentMinionsList);
        enemyLocation = minionLocation;
    }

    private void EventTakeDamage(params object[] args)
    {
        if (args[5] == enemyData)
        {
            if (enemyData.IsMinion)
            {
                BattleManager.Instance.RemoveMinion(enemyLocation);
            }
            else
            {
                for (int i = 0; i < currentMinionsList.Count; i++)
                {
                    string key = currentMinionsList.ElementAt(i).Key;
                    currentMinionsList[key].CurrentHealth = enemyData.CurrentHealth;
                }
                attackCount--;
                if (attackCount <= 0)
                {
                    BattleManager.Instance.TemporaryChangeEffect(enemy, "CreateMinionsEffect=5", BattleManager.Instance.CurrentPlayerLocation);
                    BattleManager.Instance.ClearAllMinions();
                }
            }
        }
    }
    public string SetTitleText()
    {
        return "幻象";
    }
    public string SetDescriptionText()
    {
        return "召喚數個幻象，幻象會與主敵人隨機交換位置。";
    }

}
