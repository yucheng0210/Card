using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIVictoryReward : UIBase
{
    [SerializeField]
    private Transform rewardGroupTrans;

    [SerializeField]
    private GameObject rewardPrefab;
    private Text rewardName,
        rewardCount;

    protected override void Start()
    {
        base.Start();
        EventManager.Instance.AddEventRegister(EventDefinition.eventBattleWin, EventBattleWin);
    }

    private void EventBattleWin(params object[] args)
    {
        UI.SetActive(true);
        int id = DataManager.Instance.LevelID;
        for (int i = 0; i < DataManager.Instance.LevelList[id].RewardIDList.Count; i++)
        {
            int rewardID = DataManager.Instance.LevelList[id].RewardIDList[i].Item1;
            GameObject reward = Instantiate(rewardPrefab, rewardGroupTrans);
            rewardName = reward.transform.GetChild(0).GetComponent<Text>();
            rewardCount = reward.transform.GetChild(1).GetComponent<Text>();
            rewardName.text = DataManager.Instance.ItemList[rewardID].ItemName;
            rewardCount.text =
                "X" + DataManager.Instance.LevelList[id].RewardIDList[i].Item2.ToString();
        }
    }
}
