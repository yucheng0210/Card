using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExtinctionRayEffect : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        int mana = BattleManager.Instance.CurrentPlayerData.Mana;
        int upCount = mana / 10;
        int damage = (int)(mana * 2 + 2 * Mathf.Pow(2, upCount));
        EnemyData enemyData = (EnemyData)BattleManager.Instance.IdentifyCharacter(toLocation);
        BattleManager.Instance.TakeDamage(BattleManager.Instance.CurrentPlayerData, enemyData, damage, toLocation, 0);
        BattleManager.Instance.ConsumeMana(mana);
    }

    public string SetDescriptionText()
    {
        return "毀滅射線";
    }

    public string SetTitleText()
    {
        return "";
    }
}
