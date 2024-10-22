using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckerboardSlot : MonoBehaviour
{
    [SerializeField]
    private Sprite blueClueImage;
    [SerializeField]
    private Sprite redClueImage;
    public Sprite BlueClueImage { get { return blueClueImage; } }
    public Sprite RedClueImage { get { return redClueImage; } }
}
