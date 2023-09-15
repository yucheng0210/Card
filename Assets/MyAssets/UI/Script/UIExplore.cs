using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIExplore : UIBase
{
    [SerializeField]
    private GameObject corpse;

    protected override void Start()
    {
        base.Start();
        EventManager.Instance.AddEventRegister(EventDefinition.eventExplore, EventExplore);
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
        }
    }
}
