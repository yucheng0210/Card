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
                corpse.SetActive(true);
                corpse.GetComponentInChildren<Button>().onClick.AddListener(() => EventManager.Instance.DispatchEvent(EventDefinition.eventBattleWin));
                corpse.GetComponentInChildren<Button>().onClick.AddListener(() => corpse.SetActive(false));
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
        int currentRemoveID = UIManager.Instance.CurrentRemoveID;
        UnityEngine.Events.UnityAction unityAction = () => RemoveSuccess(currentRemoveID, BattleManager.Instance.CardBagTrans.GetChild(currentRemoveID).gameObject);
        restButton.onClick.AddListener(() => restMenu.SetActive(true));
        restButton.onClick.AddListener(() => restButton.gameObject.SetActive(false));
        restButton.onClick.AddListener(() => removeCardButton.gameObject.SetActive(false));
        //removeCardButton.onClick.AddListener(() => UIManager.Instance.RefreshCardBag());
        removeCardButton.onClick.AddListener(() => UIManager.Instance.SelectCard(unityAction));
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
    private void RemoveSuccess(int removeID, GameObject removeCard)
    {
        BattleManager.Instance.CardBagApplyButton.onClick.RemoveAllListeners();
        BattleManager.Instance.CardBagApplyButton.gameObject.SetActive(false);
        DataManager.Instance.CardBag.RemoveAt(removeID);
        Destroy(removeCard);
        for (int i = 0; i < BattleManager.Instance.CardBagTrans.childCount; i++)
        {
            BattleManager.Instance.CardBagTrans.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
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
