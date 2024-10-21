using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportEffect : IEffect
{
    public void ApplyEffect(int value, string target)
    {
        throw new System.NotImplementedException();
    }
    public string SetTitleText()
    {
        return "瞬閃";
    }

    public string SetDescriptionText()
    {
        return "隨機朝遠離敵人的方向移動。";
    }

}
