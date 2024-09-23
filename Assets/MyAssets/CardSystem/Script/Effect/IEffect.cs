using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IEffect
{
    void ApplyEffect(int value, string target);
    Sprite SetIcon();
    string SetTitleText();
    string SetDescriptionText();
}
