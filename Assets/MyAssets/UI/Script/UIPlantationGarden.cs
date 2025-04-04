using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
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
    [SerializeField]
    private Transform harvestClueTrans;
    private Dictionary<Button, Item> farmList = new();
    private List<Item> isWateredFarmList = new();
    private bool isUsingTool;
    protected override void Start()
    {
        base.Start();
        Initialize();
        EventManager.Instance.AddEventRegister(EventDefinition.eventUseItem, EventUseItem);
        EventManager.Instance.AddEventRegister(EventDefinition.eventAddItemToBag, EventAddItemToBag);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && isUsingTool)
        {
            ClearButtonListener();
        }
    }
    private void Initialize()
    {
        fertilizeButton.onClick.AddListener(Fertilize);
        wateringButton.onClick.AddListener(Watering);
        harvestButton.onClick.AddListener(Harvest);
        shopButton.onClick.AddListener(() => UIManager.Instance.ShowUI("UIShop"));
        exitButton.onClick.AddListener(() => UIManager.Instance.HideUI(GetType().Name));
        for (int i = 0; i < farmButtonList.Count; i++)
        {
            farmList.Add(farmButtonList[i], null);
        }
    }
    private void Fertilize()
    {
        if (isUsingTool || !DataManager.Instance.Backpack.ContainsKey(5001))
        {
            return;
        }
        isUsingTool = true;
        Cursor.SetCursor(fertilizeTexture, Vector2.zero, CursorMode.Auto);
        for (int i = 0; i < farmList.Count; i++)
        {
            Button button = farmList.ElementAt(i).Key;
            Item item = farmList[button];
            if (item != null)
            {
                button.onClick.AddListener(() =>
                {
                    PlantGrowth(item.ItemImagePath, button, item, false);
                    BackpackManager.Instance.ReduceItem(5001, DataManager.Instance.Backpack);
                }
                );
            }
        }
    }
    private void Watering()
    {
        if (isUsingTool)
        {
            return;
        }
        isUsingTool = true;
        Cursor.SetCursor(wateringTexture, Vector2.zero, CursorMode.Auto);
        for (int i = 0; i < farmList.Count; i++)
        {
            Button button = farmList.ElementAt(i).Key;
            Item item = farmList[button];
            if (item != null)
            {
                if (isWateredFarmList.Contains(item))
                {
                    button.onClick.AddListener(() =>
                    {
                        BattleManager.Instance.ShowCharacterStatusClue(harvestClueTrans, "已經澆過水", 0);
                        ClearButtonListener();
                    }
                );
                }
                else
                {
                    button.onClick.AddListener(() =>
                    {
                        isWateredFarmList.Add(item);
                        PlantGrowth(item.ItemImagePath, button, item, false);
                    }
                );
                }
            }
        }
    }
    private void Harvest()
    {
        if (isUsingTool)
        {
            return;
        }
        isUsingTool = true;
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
        if (item.ItemType != "種子")
        {
            return;
        }
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
        Image plantImage = plant.transform.GetChild(0).GetComponent<Image>();
        plantImage.sprite = plantSprite;
        plantImage.color = Color.white;
        ClearButtonListener();
        item.ItemImagePath = plantPath;
        farmList[plant] = item;
    }
    private void HarvestListener(Button plant, Item item)
    {
        Image plantImage = plant.transform.GetChild(0).GetComponent<Image>();
        plantImage.color = new(1, 1, 1, 0);
        ClearButtonListener();
        int potionID = item.ItemID;
        Potion potion = DataManager.Instance.PotionList[potionID];
        BackpackManager.Instance.AddPotion(potionID);
        BattleManager.Instance.ShowCharacterStatusClue(harvestClueTrans, $"獲得{potion.ItemName}", 0);
        farmList[plant] = null;
    }
    private void ClearButtonListener()
    {
        for (int i = 0; i < farmList.Count; i++)
        {
            Button button = farmList.ElementAt(i).Key;
            button.onClick.RemoveAllListeners();
        }
        Cursor.SetCursor(BattleManager.Instance.DefaultCursor, BattleManager.Instance.DefaultCursorHotSpot, CursorMode.Auto);
        isUsingTool = false;
    }
    private void EventAddItemToBag(params object[] args)
    {
        fertilizeButton.GetComponentInChildren<Text>().text = DataManager.Instance.Backpack.ContainsKey(5001) ? DataManager.Instance.Backpack[5001].ItemHeld.ToString() : "0";
    }
}
