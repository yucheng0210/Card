using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
public class EffectFactory : Singleton<EffectFactory>
{
    public IEffect CreateEffect(string effectType)
    {
        Type type = Type.GetType(effectType);
        if (type == null)
        {
            Debug.LogWarning($"Effect type '{effectType}' not found.");
            return null;
        }

        // 確保類型實現了IEffect接口
        if (!typeof(IEffect).IsAssignableFrom(type))
        {
            Debug.LogWarning($"Effect type '{effectType}' does not implement IEffect interface.");
            return null;
        }

        // 創建實例
        return (IEffect)Activator.CreateInstance(type);
    }
}
