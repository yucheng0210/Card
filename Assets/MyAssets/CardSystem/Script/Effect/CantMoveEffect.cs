using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CantMoveEffect : IEffect
{
    public void ApplyEffect(int value, string target)
    {
        var negativeState = BattleManager.Instance.CurrentNegativeState;
        string effectName = GetType().Name;

        if (negativeState.ContainsKey(effectName))
            negativeState[effectName] += value;
        else
            negativeState.Add(effectName, value);

        // 發送 UI 更新事件
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    public Sprite SetIcon()
    {
        // 加載並返回效果圖標
        return Resources.Load<Sprite>("EffectImage/CantMove");
    }
    public string SetDescriptionText()
    {
        return "禁錮";
    }

    public string SetTitleText()
    {
        return "禁止移動。";
    }
}
