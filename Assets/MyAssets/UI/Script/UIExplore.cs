using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIExplore : UIBase
{
    [SerializeField]
    private GameObject corpse;

    [SerializeField]
    private Button removeCardButton;
    [SerializeField]
    private Transform contentTrans;
    [SerializeField]
    private Button applyButton;
    [SerializeField]
    private Button exitButton;
    private int currentRemoveID = -1;

    protected override void Start()
    {
        base.Start();
        EventManager.Instance.AddEventRegister(EventDefinition.eventExplore, EventExplore);
        exitButton.onClick.AddListener(ExitExplore);
    }

    private void HideAllUI()
    {
        for (int i = 1; i < UI.transform.childCount; i++)
        {
            UI.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void Dialog()
    {
        //entrance.SetActive(true);
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Dialog);
    }

    private void Random()
    {
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Dialog);
        corpse.SetActive(true);
        corpse.GetComponent<Button>().onClick.AddListener(() => EventManager.Instance.DispatchEvent(EventDefinition.eventBattleWin));
        corpse.GetComponent<Button>().onClick.AddListener(() => corpse.SetActive(false));
    }

    private void Battle()
    {
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.BattleInitial);
        UI.SetActive(false);
    }

    private void Recover()
    {
        int recoverCount = (int)(
            DataManager.Instance.PlayerList[DataManager.Instance.PlayerID].MaxHealth * 0.35f
        );
        DataManager.Instance.PlayerList[DataManager.Instance.PlayerID].CurrentHealth +=
            recoverCount;
        ;
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Dialog);
    }

    private void Boss()
    {
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Dialog);
        UI.SetActive(false);
    }
    private void RemoveCard()
    {
        for (int i = 0; i < contentTrans.childCount; i++)
        {
            int avoidClosure = i;
            Button cardButton = contentTrans.GetChild(avoidClosure).AddComponent<Button>();
            cardButton.onClick.AddListener(() => RefreshRemoveID(avoidClosure));
        }
    }
    private void RefreshRemoveID(int removeID)
    {
        applyButton.gameObject.SetActive(true);
        currentRemoveID = removeID;
        applyButton.onClick.RemoveAllListeners();
        for (int i = 0; i < contentTrans.childCount; i++)
        {
            UIManager.Instance.ChangeOutline(contentTrans.GetChild(i).GetComponentInChildren<Outline>(), 0);
        }
        if (currentRemoveID <= contentTrans.childCount && currentRemoveID != -1)
        {
            UIManager.Instance.ChangeOutline(contentTrans.GetChild(currentRemoveID).GetComponentInChildren<Outline>(), 6);
            applyButton.onClick.AddListener(() => RemoveSuccess(currentRemoveID, contentTrans.GetChild(currentRemoveID).gameObject));
        }
    }
    private void RemoveSuccess(int removeID, GameObject removeCard)
    {
        applyButton.onClick.RemoveAllListeners();
        applyButton.gameObject.SetActive(false);
        DataManager.Instance.CardBag.RemoveAt(removeID);
        Destroy(removeCard);
        for (int i = 0; i < contentTrans.childCount; i++)
        {
            contentTrans.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
        }
        removeCardButton.gameObject.SetActive(false);
        currentRemoveID = -1;
    }
    private void ExitExplore()
    {
        UIManager.Instance.ShowUI("UIMap");
        UIManager.Instance.HideUI("UICardMenu");
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }
    private void EventExplore(params object[] args)
    {
        DataManager.Instance.LevelList[DataManager.Instance.LevelID].LevelPassed = true;
        switch (args[0])
        {
            case "DIALOG":
                Dialog();
                break;
            case "RANDOM":
                Random();
                break;
            case "BATTLE":
                Battle();
                break;
            case "RECOVERY":
                Recover();
                break;
            case "BOSS":
                Boss();
                break;
            case "REMOVECARD":
                removeCardButton.gameObject.SetActive(true);
                removeCardButton.onClick.AddListener(RemoveCard);
                break;

        }
    }
}
