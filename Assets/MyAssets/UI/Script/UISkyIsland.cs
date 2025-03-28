using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkyIsland : UIBase
{
    [SerializeField]
    private Button plantationGardenButton;
    [SerializeField]
    private Button guildButton;
    [SerializeField]
    private Button shopButton;
    protected override void Start()
    {
        base.Start();
        Initialize();
        if (MapManager.Instance.LevelCount > 14)
        {
            Show();
        }
    }
    private void Initialize()
    {
        plantationGardenButton.onClick.AddListener(() => UIManager.Instance.ShowUI("UIPlantationGarden"));
        guildButton.onClick.AddListener(() => UIManager.Instance.ShowUI("UIGuild"));
        shopButton.onClick.AddListener(() => UIManager.Instance.ShowUI("UIShop"));
    }

}
