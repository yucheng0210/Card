using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffect
{
    void ApplyEffect(int value, string target);
    Sprite SetIcon();
}
