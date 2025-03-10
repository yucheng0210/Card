using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using UnityEngine.Events;

public class UIExplore : UIBase
{
    [Header("隨機事件")]
    [SerializeField] private GameObject corpse;
    [SerializeField] private Button corpseYesButton;
    [SerializeField] private Button corpseNoButton;
    [SerializeField] private GameObject corpseResult;
    [SerializeField] private Text corpseDescription;
    [SerializeField] private Button corpseConfirmButton;

    [Header("休息")]
    [SerializeField] private GameObject recoverMenu;
    [SerializeField] private Button restButton;
    [SerializeField] private Button removeCardButton;
    [SerializeField] private GameObject restMenu;
    [SerializeField] private Button restConfirmButton;
    [SerializeField] private Button recoverExitButton;

    [Header("玩家")]
    [SerializeField] private Text cardBagCountText;
    [SerializeField] private Image health;
    [SerializeField] private Text healthText;
    [SerializeField] private Text moneyText;

    [Header("寶藏")]
    [SerializeField] private GameObject treasure;
    [SerializeField] private Button treasureButton;
    [SerializeField] private GameObject openedTreasureBackground;
    [SerializeField] private Image treasureItemImage;
    [SerializeField] private Button treasureExitButton;
    protected override void Start()
    {
        base.Start();
        RegisterEvents();
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.None);
        Initialize();
    }
    private void Initialize()
    {
        restButton.onClick.AddListener(OpenRestMenu);
        removeCardButton.onClick.AddListener(OpenCardSelection);
        restConfirmButton.onClick.AddListener(OnRestConfirmed);
        recoverExitButton.onClick.AddListener(() => ExitExplore("UICardMenu"));
        corpseYesButton.onClick.AddListener(() => OnCorpseButtonClicked());
        corpseNoButton.onClick.AddListener(() => ExitExplore(GetType().Name));
        treasureButton.onClick.AddListener(OnTreasureButtonClicked);
        treasureExitButton.onClick.AddListener(() =>
        {
            ExitExplore("UIExplore");
            treasure.SetActive(false);
            openedTreasureBackground.SetActive(false);
            treasureExitButton.gameObject.SetActive(false);
        }
        );
    }
    private void RegisterEvents()
    {
        EventManager.Instance.AddEventRegister(EventDefinition.eventExplore, OnExploreEvent);
        EventManager.Instance.AddEventRegister(EventDefinition.eventRefreshUI, RefreshUI);
    }

    private void OnExploreEvent(params object[] args)
    {
        switch (args[0])
        {
            case "DIALOG":
                OpenDialog();
                break;
            case "RANDOM":
                TriggerRandomEvent();
                break;
            case "BATTLE":
                StartBattle();
                break;
            case "RECOVER":
                OpenRecoverMenu();
                break;
            case "BOSS":
                StartBossBattle();
                break;
            case "SHOP":
                OpenShop();
                break;
            case "TREASURE":
                OpenTreasure();
                break;
            case "FINALBOSS":
                StartFinalBossBattle();
                break;
        }
    }

    private void OpenDialog()
    {
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Dialog);
    }

    private void TriggerRandomEvent()
    {
        int randomIndex = Random.Range(0, 3);
        switch (randomIndex)
        {
            case 0:
                ShowCorpse();
                break;
            case 1:
                ShowCorpse();
                break;
            case 2:
                ShowCorpse();
                break;
        }
    }

    private void ShowCorpse()
    {
        UI.SetActive(true);
        corpse.SetActive(true);
    }

    private void OnCorpseButtonClicked()
    {
        int randomEvent = Random.Range(0, 5); // 修正範圍，確保 case 3, 4, 5 可執行
        PlayerData playerData = BattleManager.Instance.CurrentPlayerData;
        Dictionary<int, Potion> potionList = DataManager.Instance.PotionList;
        int effectValue = 0; // 避免影響 switch 判斷
        string description = "";
        corpseConfirmButton.onClick.RemoveAllListeners();
        UnityAction unityAction = () =>
        {
            corpseResult.SetActive(false);
            ExitExplore(GetType().Name);
        };
        switch (randomEvent)
        {
            case 0: // 獲得金幣
                unityAction += () =>
                {
                    effectValue = Random.Range(30, 70);
                    DataManager.Instance.MoneyCount += effectValue;
                };
                description = "找到少量金幣";
                break;

            case 1: // 減少生命值
                unityAction += () =>
                {
                    effectValue = Random.Range(10, 20);
                    playerData.CurrentHealth = Mathf.Max(1, playerData.CurrentHealth - effectValue); // 確保不變成負數
                };
                description = "被附近野狗咬了一口";
                break;

            case 2: // 獲得詛咒卡片
                unityAction += () =>
                {
                    BattleManager.Instance.AddCard(5004);
                };
                description = "遭受亡者的詛咒";
                break;

            case 3: // 獲得隨機藥水
                effectValue = Random.Range(0, potionList.Count);
                Potion potion = potionList.ElementAt(effectValue).Value;
                unityAction += () =>
                {
                    DataManager.Instance.PotionBag.Add(potion);
                };
                description = $"找到遺留的{potion.ItemName}";
                break;

            case 4: // 獲得祝福
                unityAction += () =>
                {
                    BattleManager.Instance.CurrentPlayerData.StartSkillList.Add(1002);
                };
                description = "獲得亡者庇蔭";
                break;
        }
        corpseResult.SetActive(true);
        corpseDescription.text = description;
        unityAction += () => EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
        corpseConfirmButton.onClick.AddListener(unityAction);
    }

    private void StartBattle()
    {
        UIManager.Instance.ShowUI("UIBattle");
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.BattleInitial);
        UI.SetActive(false);
    }

    private void StartBossBattle()
    {
        StartBattle(); // Boss戰也使用相同的流程
    }
    private void StartFinalBossBattle()
    {
        StartBattle();
    }
    private void OpenShop()
    {
        UIManager.Instance.ShowUI("UIShop");
        UI.SetActive(false);
    }

    private void OpenTreasure()
    {
        UIManager.Instance.ShowUI("UIExplore");
        treasure.SetActive(true);
    }

    private void OnTreasureButtonClicked()
    {
        Dictionary<int, Potion> potionList = DataManager.Instance.PotionList;
        int randomIndex = Random.Range(0, potionList.Count);
        Potion potion = potionList.ElementAt(randomIndex).Value;
        openedTreasureBackground.SetActive(true);
        treasureItemImage.sprite = Resources.Load<Sprite>(potion.ItemImagePath);
        DataManager.Instance.PotionBag.Add(potion);
        treasureExitButton.gameObject.SetActive(true);
    }

    private void OpenRestMenu()
    {
        restMenu.SetActive(true);
        restButton.gameObject.SetActive(false);
        removeCardButton.gameObject.SetActive(false);
    }

    private void OpenCardSelection()
    {
        UnityAction unityAction = () => RemoveCardSuccess();
        UIManager.Instance.SelectCard(unityAction, false);
    }

    private void OnRestConfirmed()
    {
        int recoverAmount = BattleManager.Instance.GetPercentage(BattleManager.Instance.CurrentPlayerData.MaxHealth, 35);
        BattleManager.Instance.Recover(BattleManager.Instance.CurrentPlayerData, recoverAmount);
        recoverExitButton.gameObject.SetActive(true);
        restMenu.SetActive(false);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    private void RemoveCardSuccess()
    {
        int cardIndex = UIManager.Instance.CurrentRemoveID;
        DataManager.Instance.CardBag.RemoveAt(cardIndex);
        BattleManager.Instance.CardBagApplyButton.gameObject.SetActive(false);
        recoverMenu.SetActive(false);
        ExitExplore("UICardMenu");
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }


    private void OpenRecoverMenu()
    {
        UI.SetActive(true);
        recoverMenu.SetActive(true);
        restButton.gameObject.SetActive(true);
        removeCardButton.gameObject.SetActive(true);
        recoverExitButton.gameObject.SetActive(false);
    }

    private void ExitExplore(string hideMenu)
    {
        BattleManager.Instance.NextLevel(hideMenu);
    }

    private void RefreshUI(params object[] args)
    {
        PlayerData playerData = BattleManager.Instance.CurrentPlayerData;
        cardBagCountText.text = DataManager.Instance.CardBag.Count.ToString();
        health.DOFillAmount((float)playerData.CurrentHealth / playerData.MaxHealth, 0.5f);
        healthText.text = $"{playerData.CurrentHealth}/{playerData.MaxHealth}";
        moneyText.text = DataManager.Instance.MoneyCount.ToString();
    }
}
