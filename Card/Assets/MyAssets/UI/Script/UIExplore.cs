using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIExplore : UIBase
{
    [SerializeField]
    private GameObject entrance;

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

    private void Entrance()
    {
        HideAllUI();
        entrance.SetActive(true);
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Dialog);
    }

    private void Random()
    {
        HideAllUI();
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Dialog);
    }

    private void EventExplore(params object[] args)
    {
        switch (args[0].ToString())
        {
            case "ENTRANCE":
                Entrance();
                break;
            case "RANDOM":
                Random();
                break;
            case "ENEMY":
                break;
            case "RECOVERY":
                break;
            case "BOSS":
                break;
        }
    }
}
