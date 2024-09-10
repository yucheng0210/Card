using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveBattleShieldEffect : IEffect
{
    private CharacterData targetCharacter;
    private bool wasAttackedLastTurn;
    private int shieldCount;

    public void ApplyEffect(int value, string target)
    {
        // 初始化狀態
        wasAttackedLastTurn = false;
        targetCharacter = BattleManager.Instance.CurrentEnemyList.ContainsKey(target)
            ? BattleManager.Instance.CurrentEnemyList[target]
            : BattleManager.Instance.CurrentPlayerData;
        shieldCount = value;

        // 監聽攻擊事件
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);

        // 監聽敵方回合結束事件
        EventManager.Instance.AddEventRegister(EventDefinition.eventEnemyTurn, EventEnemyTurn);
    }

    private void EventTakeDamage(params object[] args)
    {
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

        // 重置狀態，準備下一回合
        wasAttackedLastTurn = false;
    }

    public Sprite SetIcon()
    {
        return Resources.Load<Sprite>("EffectImage/ShieldIcon");
    }
}
