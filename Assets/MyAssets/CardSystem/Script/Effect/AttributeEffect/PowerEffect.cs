using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerEffect : IEffect
{
    private EnemyData enemyData;
    private Enemy enemy;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        enemyData = (EnemyData)BattleManager.Instance.IdentifyCharacter(fromLocation);
        enemy = enemyData.EnemyTrans.GetComponent<Enemy>();
        enemy.AdditionPower += value;
        BattleManager.Instance.AddState(enemy.EnemyOnceBattlePositiveList, GetType().Name, value);
        BattleManager.Instance.ShowCharacterStatusClue(enemy.StatusClueTrans, "提升力量", 0);
    }

    public string SetTitleText()
    {
        return "力量";
    }

    public string SetDescriptionText()
    {
        return "提升攻擊的屬性。";
    }
}
