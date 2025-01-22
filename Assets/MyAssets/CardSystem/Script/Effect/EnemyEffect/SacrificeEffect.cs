using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SacrificeEffect : IEffect
{
    private EnemyData enemyData;
    private Enemy enemy;
    private EnemyData masterEnemyData;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        if (!BattleManager.Instance.CurrentMinionsList.TryGetValue(fromLocation, out enemyData))
        {
            return;
        }
        enemy = enemyData.EnemyTrans.GetComponent<Enemy>();
        masterEnemyData = enemy.MasterEnemyData;
        string masterLocation = BattleManager.Instance.GetEnemyKey(masterEnemyData);
        BattleManager.Instance.Recover(masterEnemyData, enemyData.CurrentHealth, masterLocation);
        enemyData.CurrentHealth = 0;
        BattleManager.Instance.TakeDamage(enemyData, enemyData, -1, fromLocation, 1f);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }
    public string SetTitleText()
    {
        return "獻祭";
    }
    public string SetDescriptionText()
    {
        return "當血量過低時，會將剩餘的血量值回復給召喚者並死亡。";
    }

}
