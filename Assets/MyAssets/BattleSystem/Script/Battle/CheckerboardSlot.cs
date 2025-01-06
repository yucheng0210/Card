using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CheckerboardSlot : MonoBehaviour
{
    [SerializeField]
    private Sprite blueClueImage;
    [SerializeField]
    private Sprite redClueImage;
    [SerializeField]
    private Image safeZoneImage;
    public Sprite BlueClueImage { get { return blueClueImage; } }
    public Sprite RedClueImage { get { return redClueImage; } }
    public Image SafeZoneImage { get { return safeZoneImage; } set { safeZoneImage = value; } }
}
