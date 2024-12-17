using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhantomEffect : IEffect
{
    private EnemyData enemyData;
    private int attackCount = 2;
    private string enemyLocation;
    private Dictionary<string, EnemyData> currentMinionsList;
    private Enemy enemy;
    private int phantomCount;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        currentMinionsList = BattleManager.Instance.CurrentMinionsList;
        enemyData = (EnemyData)BattleManager.Instance.IdentifyCharacter(fromLocation);
        enemy = enemyData.EnemyTrans.GetComponent<Enemy>();
        enemyLocation = fromLocation;
        enemyData.PassiveSkills.Remove(GetType().Name);
        phantomCount = value;
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
        if (enemyData.IsMinion)
        {
            return;
        }
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
        EventManager.Instance.AddEventRegister(EventDefinition.eventEnemyTurn, EventEnemyTurn);
        //BattleManager.Instance.AddMinions(2009, value, fromLocation);
    }
    private void EventPlayerTurn(params object[] args)
    {
        attackCount = 2;
        int randomIndex = Random.Range(0, currentMinionsList.Count);
        string minionLocation = currentMinionsList.ElementAt(randomIndex).Key;
        Vector3 tempPosition = enemyData.EnemyTrans.localPosition;
        enemyData.EnemyTrans.localPosition = currentMinionsList[minionLocation].EnemyTrans.localPosition;
        currentMinionsList[minionLocation].EnemyTrans.localPosition = tempPosition;
        BattleManager.Instance.Replace(BattleManager.Instance.CurrentEnemyList, enemyLocation, minionLocation);
        BattleManager.Instance.Replace(currentMinionsList, minionLocation, enemyLocation);
        enemyLocation = minionLocation;
    }
    private void EventEnemyTurn(params object[] args)
    {
        if (enemy.IsDizziness)
        {
            BattleManager.Instance.AddMinions(2009, phantomCount, enemyLocation);
            enemy.IsDizziness = false;
        }
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
                    enemy.IsDizziness = true;
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
        return "召喚幻象。";
    }

}
