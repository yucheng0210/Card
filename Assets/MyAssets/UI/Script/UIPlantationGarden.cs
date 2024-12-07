using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIPlantationGarden : UIBase
{
    [Header("工具按鈕")]
    [SerializeField]
    private Button fertilizeButton;
    [SerializeField]
    private Button harvestButton;
    [SerializeField]
    private Button wateringButton;
    [SerializeField]
    private Button shopButton;
    [Header("工具圖片")]
    [SerializeField]
    private Texture2D fertilizeTexture;
    [SerializeField]
    private Texture2D harvestTexture;
    [SerializeField]
    private Texture2D wateringTexture;
    [SerializeField]
    private Texture2D shopTexture;
    protected override void Start()
    {
        base.Start();
        Initialize();
    }
    private void Initialize()
    {
        fertilizeButton.onClick.AddListener(Fertilize);
    }
    private void SetCursorTexture(Texture2D texture2D)
    {
        Cursor.SetCursor(texture2D, Vector2.zero, CursorMode.Auto);
    }
    private void Fertilize()
    {
        SetCursorTexture(fertilizeTexture);
    }
}
