using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuriousEffect : IEffect
{
    private int attackIncreaseCount;
    private EnemyData enemyData;
    private Dictionary<string, EnemyData> enemyList;

    public void ApplyEffect(int value, string target)
    {
        enemyList = BattleManager.Instance.CurrentEnemyList;

        if (enemyList.TryGetValue(target, out enemyData))
        {
            attackIncreaseCount = Mathf.RoundToInt(enemyData.CurrentAttack * (value / 100f));
            // 移除被動技能
            enemyData.PassiveSkills.Remove(GetType().Name);

            // 註冊事件
            EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
        }
    }

    private void EventTakeDamage(params object[] args)
    {
        if (!enemyList.ContainsValue(enemyData))
        {
            // 移除事件監聽
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
            return;
        }
        if (args.Length < 5)
            return;
        // 獲取攻擊者
        CharacterData attacker = (CharacterData)args[4];

        // 如果攻擊者是目標敵人，增加攻擊力
        if (enemyData == attacker)
        {
            enemyData.CurrentAttack += attackIncreaseCount;
        }
    }

    public string SetTitleText()
    {
        return "狂怒";
    }

    public string SetDescriptionText()
    {
        return "攻擊命中敵人時，提升自身的攻擊力。";
    }
}
