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
    [Header("農地")]
    [SerializeField]
    private List<Button> farmList = new List<Button>();
    [SerializeField]
    private Dictionary<int, Sprite> plantList = new();
    protected override void Start()
    {
        base.Start();
        Initialize();
        EventManager.Instance.AddEventRegister(EventDefinition.eventUseItem, EventUseItem);
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
    private void EventUseItem(params object[] args)
    {
        Item item = (Item)args[0];
        for (int i = 0; i < farmList.Count; i++)
        {
            string plantPath = item.ItemImagePath.Substring(0, item.ItemImagePath.Length - 1) + "1";
            Sprite plantSprite = Resources.Load<Sprite>(plantPath);
            Image plantImage = farmList[i].GetComponent<Image>();
            plantImage.sprite = plantSprite;
            farmList[i].onClick.AddListener(() => PlantGrowth(plantImage));
        }
    }
    private void PlantGrowth(Image plantImage)
    {
        plantImage.color = Color.white;
        for (int i = 0; i < farmList.Count; i++)
        {
            farmList[i].onClick.RemoveAllListeners();
        }
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
