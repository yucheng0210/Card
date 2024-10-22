using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportEffect : IEffect
{
    private CharacterData characterData;
    public void ApplyEffect(int value, string target)
    {
        characterData = BattleManager.Instance.IdentifyCharacter(target);
    }
    public string SetTitleText()
    {
        return "瞬閃";
    }

    public string SetDescriptionText()
    {
        return "隨機朝遠離敵人的方向瞬移。";
    }

}
