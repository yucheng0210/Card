using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIExplore : UIBase
{
    [Header("隨機事件")]
    [SerializeField] private GameObject corpse;
    [SerializeField] private Button corpseButton;
    [SerializeField] private GameObject recoverMenu;

    [Header("休息")]
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
    [SerializeField] private Transform treasureItemTrans;
    [SerializeField] private Button treasureExitButton;
    protected override void Start()
    {
        base.Start();
        RegisterEvents();
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.None);
        InitializeRecovery();
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
        /*UI.SetActive(true);
        corpse.SetActive(true);
        corpseButton.onClick.AddListener(OnCorpseButtonClicked);*/
    }

    private void OnCorpseButtonClicked()
    {
        EventManager.Instance.DispatchEvent(EventDefinition.eventBattleWin);
        corpse.SetActive(false);
        corpseButton.onClick.RemoveAllListeners();
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
        treasureButton.onClick.AddListener(OnTreasureButtonClicked);
        treasureExitButton.onClick.AddListener(() => ExitExplore("UIExplore"));
    }

    private void OnTreasureButtonClicked()
    {
        //EventManager.Instance.DispatchEvent(EventDefinition.eventBattleWin);
        openedTreasureBackground.SetActive(true);
        DataManager.Instance.PotionBag.Add(DataManager.Instance.PotionList[1001]);
        treasureExitButton.gameObject.SetActive(true);
    }

    private void InitializeRecovery()
    {
        restButton.onClick.AddListener(OpenRestMenu);
        removeCardButton.onClick.AddListener(OpenCardSelection);
        restConfirmButton.onClick.AddListener(OnRestConfirmed);
        recoverExitButton.onClick.AddListener(() => ExitExplore("UICardMenu"));
    }

    private void OpenRestMenu()
    {
        restMenu.SetActive(true);
        restButton.gameObject.SetActive(false);
        removeCardButton.gameObject.SetActive(false);
    }

    private void OpenCardSelection()
    {
        UnityEngine.Events.UnityAction unityAction = () => RemoveCardSuccess();
        UIManager.Instance.SelectCard(unityAction, false);
    }

    private void OnRestConfirmed()
    {
        int recoverAmount = (int)(BattleManager.Instance.CurrentPlayerData.MaxHealth * 0.35f);
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
