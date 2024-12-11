using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [Header("農地")]
    [SerializeField]
    private List<Button> farmButtonList = new();
    [SerializeField]
    private Button exitButton;
    private Dictionary<Button, Item> farmList = new();
    protected override void Start()
    {
        base.Start();
        Initialize();
        EventManager.Instance.AddEventRegister(EventDefinition.eventUseItem, EventUseItem);
    }
    private void Initialize()
    {
        fertilizeButton.onClick.AddListener(Fertilize);
        wateringButton.onClick.AddListener(Watering);
        harvestButton.onClick.AddListener(Harvest);
        shopButton.onClick.AddListener(() => UIManager.Instance.ShowUI("UIShop"));
        exitButton.onClick.AddListener(() => UIManager.Instance.ShowUI(GetType().Name));
        for (int i = 0; i < farmButtonList.Count; i++)
        {
            farmList.Add(farmButtonList[i], null);
        }
    }
    private void Fertilize()
    {
        Cursor.SetCursor(fertilizeTexture, Vector2.zero, CursorMode.Auto);
        for (int i = 0; i < farmList.Count; i++)
        {
            Button button = farmList.ElementAt(i).Key;
            Item item = farmList[button];
            if (item != null)
            {
                button.onClick.AddListener(() => PlantGrowth(item.ItemImagePath, button, item, false));
            }
        }
    }
    private void Watering()
    {
        Cursor.SetCursor(wateringTexture, Vector2.zero, CursorMode.Auto);
        for (int i = 0; i < farmList.Count; i++)
        {
            Button button = farmList.ElementAt(i).Key;
            Item item = farmList[button];
            if (item != null)
            {
                button.onClick.AddListener(() => PlantGrowth(item.ItemImagePath, button, item, false));
            }
        }
    }
    private void Harvest()
    {
        Cursor.SetCursor(harvestTexture, Vector2.zero, CursorMode.Auto);
        for (int i = 0; i < farmList.Count; i++)
        {
            Button button = farmList.ElementAt(i).Key;
            Item item = farmList[button];
            if (item != null)
            {
                char lastChar = item.ItemImagePath[^1];
                int lastNumber = lastChar - '0';
                if (lastNumber == 3)
                {
                    button.onClick.AddListener(() => HarvestListener(button, item));
                }
            }
        }
    }
    private void EventUseItem(params object[] args)
    {
        Item item = (Item)args[0];
        for (int i = 0; i < farmList.Count; i++)
        {
            Button button = farmList.ElementAt(i).Key;
            button.onClick.AddListener(() => PlantGrowth(item.ItemImagePath, button, item, true));
        }
    }
    private void PlantGrowth(string plantPath, Button plant, Item item, bool isPlanting)
    {
        char lastChar = plantPath[^1];
        int lastNumber = lastChar - '0';
        if (isPlanting)
        {
            lastNumber = 1;
        }
        else
        {
            if (lastNumber < 3)
            {
                lastNumber++;
            }
        }
        plantPath = plantPath.Substring(0, plantPath.Length - 1);
        plantPath += lastNumber.ToString();
        Sprite plantSprite = Resources.Load<Sprite>(plantPath);
        Image plantImage = plant.GetComponent<Image>();
        plantImage.sprite = plantSprite;
        plantImage.color = Color.white;
        for (int i = 0; i < farmList.Count; i++)
        {
            Button button = farmList.ElementAt(i).Key;
            button.onClick.RemoveAllListeners();
        }
        item.ItemImagePath = plantPath;
        farmList[plant] = item;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
    private void HarvestListener(Button plant, Item item)
    {
        Image plantImage = plant.GetComponent<Image>();
        plantImage.color = new(1, 1, 1, 0);
        for (int i = 0; i < farmList.Count; i++)
        {
            Button button = farmList.ElementAt(i).Key;
            button.onClick.RemoveAllListeners();
        }
        int potionID = item.ItemID - 1000;
        Potion potion = DataManager.Instance.PotionList[potionID];
        DataManager.Instance.PotionBag.Add(potion);
        farmList[plant] = null;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
