using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseHealth : IEffect
{
    public void ApplyEffect(int value, string target)
    {
        // 确定目标数据
        CharacterData recoverData = GetTargetCharacterData(target);

        // 计算恢复的生命值
        int recoveryAmount = Mathf.RoundToInt(recoverData.MaxHealth * (value / 100f));

        // 执行恢复操作
        BattleManager.Instance.Recover(recoverData, recoveryAmount, BattleManager.Instance.CurrentLocationID);
    }

    private CharacterData GetTargetCharacterData(string target)
    {
        // 根据目标位置获取角色数据
        if (target == BattleManager.Instance.CurrentLocationID)
            return BattleManager.Instance.CurrentPlayerData;

        // 尝试获取敌人数据
        if (BattleManager.Instance.CurrentEnemyList.TryGetValue(target, out var enemyData))
            return enemyData;

        // 如果目标不合法，记录警告并返回 null
        Debug.LogWarning($"Target '{target}' not found in CurrentEnemyList.");
        return null;
    }

    public Sprite SetIcon()
    {
        throw new System.NotImplementedException();
    }
}
