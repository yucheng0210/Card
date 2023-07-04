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

    [SerializeField]
    private Button skipButton;
    private Text rewardName,
        rewardCount;

    private void Awake()
    {
        skipButton.onClick.AddListener(NextLevel);
    }

    protected override void Start()
    {
        base.Start();
        EventManager.Instance.AddEventRegister(EventDefinition.eventBattleWin, EventBattleWin);
    }

    private void NextLevel()
    {
        UIManager.Instance.UIDict["UIBattle"].UI.SetActive(false);
        UIManager.Instance.UIDict["UIMap"].UI.SetActive(true);
    }

    private void GetReward(int count)
    {
        count--;
        if (count == 0)
            NextLevel();
        gameObject.SetActive(false);
    }

    private void GetCardReward(int count)
    {
        count--;
    }

    private void EventBattleWin(params object[] args)
    {
        UI.SetActive(true);
        int id = DataManager.Instance.LevelID;
        int totalCount =
            DataManager.Instance.LevelList[id].RewardIDList.Count
            + DataManager.Instance.LevelList[id].CardIDList.Count;
        for (int i = 0; i < DataManager.Instance.LevelList[id].RewardIDList.Count; i++)
        {
            int rewardID = DataManager.Instance.LevelList[id].RewardIDList[i].Item1;
            GameObject reward = Instantiate(rewardPrefab, rewardGroupTrans);
            rewardName = reward.transform.GetChild(0).GetComponent<Text>();
            rewardCount = reward.transform.GetChild(1).GetComponent<Text>();
            rewardName.text = DataManager.Instance.ItemList[rewardID].ItemName;
            rewardCount.text =
                "X" + DataManager.Instance.LevelList[id].RewardIDList[i].Item2.ToString();
            DataManager.Instance.Backpack.Add(rewardID, DataManager.Instance.ItemList[rewardID]);
            reward.GetComponent<Button>().onClick.AddListener(() => GetReward(totalCount));
        }
        for (int i = 0; i < DataManager.Instance.LevelList[id].CardIDList.Count; i++)
        {
            int rewardID = DataManager.Instance.LevelList[id].CardIDList[i].Item1;
            GameObject reward = Instantiate(rewardPrefab, rewardGroupTrans);
            rewardName = reward.transform.GetChild(0).GetComponent<Text>();
            rewardCount = reward.transform.GetChild(1).GetComponent<Text>();
            rewardName.text = DataManager.Instance.ItemList[rewardID].ItemName;
            rewardCount.text =
                "X" + DataManager.Instance.LevelList[id].RewardIDList[i].Item2.ToString();
            DataManager.Instance.CardBag.Add(DataManager.Instance.CardList[rewardID]);
            reward.GetComponent<Button>().onClick.AddListener(() => GetReward(totalCount));
        }
    }
}
