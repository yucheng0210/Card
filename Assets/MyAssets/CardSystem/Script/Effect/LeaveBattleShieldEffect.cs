using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaveBattleShieldEffect : IEffect
{
    private CharacterData targetCharacter;
    private bool wasAttackedLastTurn;
    private int shieldCount;
    private Dictionary<string, EnemyData> currentEnemyList;
    public void ApplyEffect(int value, string target)
    {
        // 初始化狀態
        wasAttackedLastTurn = false;
        targetCharacter = currentEnemyList.ContainsKey(target) ? currentEnemyList[target] : BattleManager.Instance.CurrentPlayerData;
        shieldCount = value;
        if (currentEnemyList.ContainsKey(target))
            currentEnemyList[target].PassiveSkills.Remove(GetType().Name);
        Debug.Log(wasAttackedLastTurn);
        // 監聽攻擊事件
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);

        // 監聽敵方回合結束事件
        EventManager.Instance.AddEventRegister(EventDefinition.eventEnemyTurn, EventEnemyTurn);
    }

    private void EventTakeDamage(params object[] args)
    {
        if (args.Length < 6)
            return;
        CharacterData attackedCharacterID = (CharacterData)args[5];

        // 如果這個角色被攻擊了，更新狀態
        if (attackedCharacterID == targetCharacter)
            wasAttackedLastTurn = true;
    }

    private void EventEnemyTurn(params object[] args)
    {
        // 如果這個回合沒有被攻擊，給予護盾
        if (!wasAttackedLastTurn)
            BattleManager.Instance.GetShield(targetCharacter, shieldCount);
        Debug.Log(wasAttackedLastTurn);
        // 重置狀態，準備下一回合
        wasAttackedLastTurn = false;
    }

    public string SetTitleText()
    {
        throw new System.NotImplementedException();
    }

    public string SetDescriptionText()
    {
        throw new System.NotImplementedException();
    }
}
