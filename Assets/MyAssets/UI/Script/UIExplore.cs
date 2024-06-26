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
    private Button removeCardButton;
    [SerializeField]
    private CardItem cardPrefab;
    [SerializeField]
    private Transform contentTrans;
    [SerializeField]
    private Button applyButton;
    [SerializeField]
    private Button exitButton;
    [SerializeField]
    private Button restButton;
    [SerializeField]
    private Button recoverExitButton;
    [SerializeField]
    private int currentRemoveID = -1;

    protected override void Start()
    {
        base.Start();
        EventManager.Instance.AddEventRegister(EventDefinition.eventExplore, EventExplore);
        exitButton.onClick.AddListener(ExitExplore);
        recoverExitButton.onClick.AddListener(ExitExplore);
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.None);
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
                break;
            case 1:
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

    private void Recover()
    {
        int playerID = DataManager.Instance.PlayerID;
        int recoverCount = (int)(DataManager.Instance.PlayerList[playerID].MaxHealth * 0.35f);
        UI.SetActive(true);
        recoverMenu.SetActive(true);
        restButton.onClick.AddListener(() => DataManager.Instance.PlayerList[playerID].CurrentHealth += recoverCount);
        restButton.onClick.AddListener(() => recoverExitButton.gameObject.SetActive(true));
        restButton.onClick.AddListener(() => EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI));
    }

    private void Boss()
    {
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Dialog);
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
        exitButton.gameObject.SetActive(true);
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
    }
    private void ExitExplore()
    {
        exitButton.gameObject.SetActive(false);
        recoverExitButton.gameObject.SetActive(false);
        UIManager.Instance.ShowUI("UIMap");
        UIManager.Instance.HideUI("UICardMenu");
        recoverMenu.SetActive(false);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }
    private void Shop()
    {
        UIManager.Instance.ShowUI("UIShop");
        UI.SetActive(false);
    }
    private void EventExplore(params object[] args)
    {
        int levelID = MapManager.Instance.LevelID;
        /*  bool isSelectLevel = true;
          for (int i = 0; i < DataManager.Instance.LevelList.Count; i++)
          {
              int id = DataManager.Instance.LevelList.ElementAt(i).Key;
              for (int j = 0; j < DataManager.Instance.LevelList[id].LevelParentList.Count; j++)
              {
                  if (DataManager.Instance.LevelList[id].LevelParentList.Count != DataManager.Instance.LevelList[levelID].LevelParentList.Count)
                  {
                      isSelectLevel = false;
                      break;
                  }
                  if (DataManager.Instance.LevelList[id].LevelParentList[j] != DataManager.Instance.LevelList[levelID].LevelParentList[j])
                      isSelectLevel = false;
              }
              if (isSelectLevel)
                  DataManager.Instance.LevelList[id].LevelPassed = true;
              isSelectLevel = true;
          }*/
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
            case "REMOVECARD":
                removeCardButton.gameObject.SetActive(true);
                currentRemoveID = -1;
                removeCardButton.onClick.AddListener(() => StartCoroutine(RemoveCard()));
                break;
            case "SHOP":
                Shop();
                break;

        }
    }
}
