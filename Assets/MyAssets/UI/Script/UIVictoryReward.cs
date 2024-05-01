using System.Linq;
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

    [SerializeField]
    private CardItem cardPrefab;

    [SerializeField]
    private GameObject cardRewardMenu;

    [SerializeField]
    private Transform cardRewardGroupTrans;
    private int totalCount;
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
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Dialog);
        UIManager.Instance.ShowUI("UIExplore");
        UI.SetActive(false);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    private void GetReward(int rewardID, GameObject reward)
    {
        BackpackManager.Instance.AddItem(rewardID, DataManager.Instance.Backpack);
        ReduceCount();
        Destroy(reward);
    }

    private void GetCardReward(GameObject reward)
    {
        List<int> normalCardList = new();
        List<int> rareCardList = new();
        for (int i = 0; i < DataManager.Instance.CardList.Count; i++)
        {
            switch (DataManager.Instance.CardList.ElementAt(i).Value.CardRarity)
            {
                case "普通":
                    normalCardList.Add(DataManager.Instance.CardList.ElementAt(i).Key);
                    break;
                case "稀有":
                    rareCardList.Add(DataManager.Instance.CardList.ElementAt(i).Key);
                    break;
            }
        }

        for (int i = 0; i < 3; i++)
        {
            int cardType = Random.Range(0, 100);
            int rewardID;
            if (cardType >= 95)
                continue;
            else if (cardType >= 80)
                rewardID = rareCardList[Random.Range(0, rareCardList.Count)];
            else
                rewardID = normalCardList[Random.Range(0, normalCardList.Count)];
            Dictionary<int, CardData> cardList = DataManager.Instance.CardList;
            cardRewardMenu.SetActive(true);
            CardItem cardItem = Instantiate(cardPrefab, cardRewardGroupTrans);
            Button cardButton = cardItem.gameObject.AddComponent<Button>();
            cardItem.GetComponent<CanvasGroup>().alpha = 1;
            cardItem.CardID = cardList[rewardID].CardID;
            cardItem.CantMove = true;
            cardButton.onClick.AddListener(() => AddCard(rewardID));
            normalCardList.Remove(rewardID);
        }
        Destroy(reward);
    }

    private void AddCard(int rewardID)
    {
        DataManager.Instance.CardBag.Add(DataManager.Instance.CardList[rewardID]);
        for (int i = 0; i < cardRewardGroupTrans.childCount; i++)
        {
            Destroy(cardRewardGroupTrans.GetChild(i).gameObject);
        }
        cardRewardMenu.SetActive(false);
        ReduceCount();
    }

    private void ReduceCount()
    {
        totalCount--;
        if (totalCount == 0)
            NextLevel();
    }

    private void EventBattleWin(params object[] args)
    {
        UI.SetActive(true);
        cardRewardMenu.SetActive(false);
        int id = MapManager.Instance.LevelID;
        totalCount = DataManager.Instance.LevelList[id].RewardIDList.Count + 1;
        for (int i = 0; i < rewardGroupTrans.childCount; i++)
        {
            Destroy(rewardGroupTrans.GetChild(i).gameObject);
        }
        for (int i = 0; i < DataManager.Instance.LevelList[id].RewardIDList.Count; i++)
        {
            int rewardID = DataManager.Instance.LevelList[id].RewardIDList[i].Item1;
            GameObject reward = Instantiate(rewardPrefab, rewardGroupTrans);
            rewardName = reward.transform.GetChild(0).GetComponent<Text>();
            rewardCount = reward.transform.GetChild(1).GetComponent<Text>();
            rewardName.text = DataManager.Instance.ItemList[rewardID].ItemName;
            rewardCount.text =
                "X" + DataManager.Instance.LevelList[id].RewardIDList[i].Item2.ToString();
            reward.GetComponent<Button>().onClick.AddListener(() => GetReward(rewardID, reward));
        }
        GameObject cardReward = Instantiate(rewardPrefab, rewardGroupTrans);
        rewardName = cardReward.transform.GetChild(0).GetComponent<Text>();
        rewardName.text = "卡包";
        cardReward.GetComponent<Button>().onClick.AddListener(() => GetCardReward(cardReward));
    }
}
