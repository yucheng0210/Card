using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIExplore : UIBase
{
    [Header("隨機事件")]
    [SerializeField]
    private GameObject corpse;
    [SerializeField]
    private GameObject recoverMenu;
    [SerializeField]
    private CardItem cardPrefab;
    [SerializeField]
    private Transform contentTrans;
    [SerializeField]
    private Button applyButton;
    [SerializeField]
    private Button exitButton;
    [Header("休息")]
    [SerializeField]
    private Button restButton;
    [SerializeField]
    private Button removeCardButton;
    [SerializeField]
    private GameObject restMenu;
    [SerializeField]
    private Button restConfirmButton;
    [SerializeField]
    private Button recoverExitButton;
    [SerializeField]
    private int currentRemoveID = -1;

    protected override void Start()
    {
        base.Start();
        EventManager.Instance.AddEventRegister(EventDefinition.eventExplore, EventExplore);
        // exitButton.onClick.AddListener(ExitExplore);
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.None);
        RecoverInitialize();
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
        //BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Dialog);
        /*corpse.SetActive(true);
        corpse.GetComponent<Button>().onClick.AddListener(() => EventManager.Instance.DispatchEvent(EventDefinition.eventBattleWin));
        corpse.GetComponent<Button>().onClick.AddListener(() => corpse.SetActive(false));*/
        int randomIndex = UnityEngine.Random.Range(0, 3);
        switch (randomIndex)
        {
            case 0:
                Recover();
                break;
            case 1:
                Battle();
                break;
            case 2:
                break;
        }
    }

    private void Battle()
    {
        //BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Dialog);
        UIManager.Instance.ShowUI("UIBattle");
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.BattleInitial);
        UI.SetActive(false);
    }
    private void RecoverInitialize()
    {
        int playerID = DataManager.Instance.PlayerID;
        int recoverCount = (int)(DataManager.Instance.PlayerList[playerID].MaxHealth * 0.35f);
        restButton.onClick.AddListener(() => restMenu.SetActive(true));
        restButton.onClick.AddListener(() => restButton.gameObject.SetActive(false));
        restButton.onClick.AddListener(() => removeCardButton.gameObject.SetActive(false));
        removeCardButton.onClick.AddListener(() => StartCoroutine(RemoveCard()));
        restConfirmButton.onClick.AddListener(() => DataManager.Instance.PlayerList[playerID].CurrentHealth += recoverCount);
        restConfirmButton.onClick.AddListener(() => recoverExitButton.gameObject.SetActive(true));
        restConfirmButton.onClick.AddListener(() => restMenu.SetActive(false));
        restConfirmButton.onClick.AddListener(() => EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI));
        recoverExitButton.onClick.AddListener(() => recoverMenu.SetActive(false));
        recoverExitButton.onClick.AddListener(ExitExplore);
    }
    private void Recover()
    {
        UI.SetActive(true);
        recoverMenu.SetActive(true);
        restButton.gameObject.SetActive(true);
        removeCardButton.gameObject.SetActive(true);
    }

    private void Boss()
    {
        UIManager.Instance.ShowUI("UIBattle");
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.BattleInitial);
        UI.SetActive(false);
    }
    private IEnumerator RemoveCard()
    {
        UIManager.Instance.RefreshCardBag(contentTrans, cardPrefab);
        yield return null;
        for (int i = 0; i < contentTrans.childCount; i++)
        {
            int avoidClosure = i;
            Button cardButton = contentTrans.GetChild(i).AddComponent<Button>();
            cardButton.onClick.AddListener(() => RefreshRemoveID(avoidClosure));
        }
    }
    private void RefreshRemoveID(int removeID)
    {
        applyButton.gameObject.SetActive(true);
        // exitButton.gameObject.SetActive(true);
        currentRemoveID = removeID;
        applyButton.onClick.RemoveAllListeners();
        for (int i = 0; i < contentTrans.childCount; i++)
        {
            UIManager.Instance.ChangeOutline(contentTrans.GetChild(i).GetComponentInChildren<Outline>(), 0);
        }
        if (currentRemoveID < contentTrans.childCount && currentRemoveID != -1)
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
        removeCardButton.onClick.RemoveAllListeners();
        removeCardButton.gameObject.SetActive(false);
        recoverMenu.SetActive(false);
        ExitExplore();
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }
    private void ExitExplore()
    {
        BattleManager.Instance.NextLevel("UICardMenu");
    }
    private void Shop()
    {
        UIManager.Instance.ShowUI("UIShop");
        UI.SetActive(false);
    }
    private void EventExplore(params object[] args)
    {
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
            case "RECOVER":
                Recover();
                break;
            case "BOSS":
                Boss();
                break;

            case "SHOP":
                Shop();
                break;

        }
    }
}
