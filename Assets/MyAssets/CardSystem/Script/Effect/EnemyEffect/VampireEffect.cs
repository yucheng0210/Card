using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class VampireEffect : IEffect
{
    private CharacterData characterData;
    private float vampireMultiplier;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        characterData = BattleManager.Instance.IdentifyCharacter(fromLocation);
        vampireMultiplier = value / 100f;
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
    }
    private void EventTakeDamage(params object[] args)
    {
        if (args.Length > 6 && args[4] == characterData)
        {
            int recoverCount = Mathf.RoundToInt((int)args[6] * vampireMultiplier);
            BattleManager.Instance.Recover(characterData, recoverCount);
        }
    }
    public string SetTitleText()
    {
        return "吸血";
    }
    public string SetDescriptionText()
    {
        return "";
    }

}
