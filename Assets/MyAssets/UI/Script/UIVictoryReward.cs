using System.Linq;
using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class UIVictoryReward : UIBase
{
    [SerializeField]
    private Transform rewardGroupTrans;

    [SerializeField]
    private GameObject rewardPrefab;
    [SerializeField]
    private GameObject moneyRewardPrefab;

    [SerializeField]
    private Button skipButton;

    [SerializeField]
    private CardItem cardPrefab;

    [SerializeField]
    private GameObject cardRewardMenu;

    [SerializeField]
    private Transform cardRewardGroupTrans;
    private int totalCount;

    private void Awake()
    {
        skipButton.onClick.AddListener(() => BattleManager.Instance.NextLevel("UIBattle"));
        skipButton.onClick.AddListener(() => UIManager.Instance.HideUI("UIVictoryReward"));
    }

    protected override void Start()
    {
        base.Start();
        EventManager.Instance.AddEventRegister(EventDefinition.eventBattleWin, EventBattleWin);
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
            if (DataManager.Instance.CardList.ElementAt(i).Value.CardType == "詛咒")
            {
                continue;
            }
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
            {
                i--;
                continue;
            }
            else if (cardType >= 80)
            {
                rewardID = rareCardList[Random.Range(0, rareCardList.Count)];
            }
            else
            {
                rewardID = normalCardList[Random.Range(0, normalCardList.Count)];
            }
            Dictionary<int, CardData> cardList = DataManager.Instance.CardList;
            CardItem cardItem = Instantiate(cardPrefab, cardRewardGroupTrans);
            Button cardButton = cardItem.CardImage.gameObject.AddComponent<Button>();
            UIManager.Instance.ChangeOutline(cardItem, false);
            cardRewardMenu.SetActive(true);
            cardItem.CardImage.raycastTarget = true;
            cardItem.GetComponent<CanvasGroup>().alpha = 1;
            cardItem.MyCardData = cardList[rewardID];
            cardItem.CantMove = true;
            cardButton.onClick.AddListener(() => AddCard(rewardID));
            normalCardList.Remove(rewardID);
        }
        Destroy(reward);
    }
    private void GetDropMoney(GameObject moneyReward, int totalMoneyCount)
    {
        DataManager.Instance.MoneyCount += totalMoneyCount;
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
        Destroy(moneyReward);
        ReduceCount();
    }
    private void GetTalentSkillReward(GameObject reward, int skillIndex)
    {
        for (int i = 0; i < BattleManager.Instance.CurrentPlayerData.StartSkillList.Count; i++)
        {
            int skillID = BattleManager.Instance.CurrentPlayerData.StartSkillList[i];
            if (DataManager.Instance.SkillList[skillID].IsTalentSkill)
            {
                BattleManager.Instance.CurrentPlayerData.StartSkillList.Remove(skillID);
            }
        }
        BattleManager.Instance.CurrentPlayerData.StartSkillList.Add(skillIndex);
        BattleManager.Instance.PlayerAni.runtimeAnimatorController = DataManager.Instance.SkillList[skillIndex].TalentAnimatorController;
        Destroy(reward);
    }
    private void AddCard(int rewardID)
    {
        BattleManager.Instance.AddCard(rewardID);
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
        {
            BattleManager.Instance.NextLevel("UIBattle");
            UIManager.Instance.HideUI("UIVictoryReward");
        }
    }

    private void EventBattleWin(params object[] args)
    {
        UI.SetActive(true);
        cardRewardMenu.SetActive(false);
        int id = MapManager.Instance.LevelID;
        int count = MapManager.Instance.LevelCount;
        totalCount = MapManager.Instance.MapNodes[count][id].l.RewardIDList.Count + 2;
        for (int i = 0; i < rewardGroupTrans.childCount; i++)
        {
            Destroy(rewardGroupTrans.GetChild(i).gameObject);
        }
        for (int i = 0; i < MapManager.Instance.MapNodes[count][id].l.RewardIDList.Count; i++)
        {
            int rewardID = MapManager.Instance.MapNodes[count][id].l.RewardIDList[i].Item1;
            GameObject reward = Instantiate(rewardPrefab, rewardGroupTrans);
            reward.GetComponent<Image>().sprite = Resources.Load<Sprite>(DataManager.Instance.ItemList[rewardID].ItemImagePath);
            reward.GetComponent<Button>().onClick.AddListener(() => GetReward(rewardID, reward));
        }
        GameObject cardReward = Instantiate(rewardPrefab, rewardGroupTrans);
        cardReward.GetComponent<Button>().onClick.AddListener(() => GetCardReward(cardReward));
        GameObject moneyReward = Instantiate(moneyRewardPrefab, rewardGroupTrans);
        int totalMoneyCount = 0;
        for (int i = 0; i < MapManager.Instance.MapNodes[count][id].l.EnemyIDList.Count; i++)
        {
            int enemyID = MapManager.Instance.MapNodes[count][id].l.EnemyIDList.ElementAt(i).Value;
            int minMoneyCount = Mathf.RoundToInt(DataManager.Instance.EnemyList[enemyID].DropMoney / 5f * 4f);
            int maxMoneyCount = Mathf.RoundToInt(DataManager.Instance.EnemyList[enemyID].DropMoney / 5f * 6f);
            int randomMoney = Random.Range(minMoneyCount, maxMoneyCount);
            totalMoneyCount += randomMoney;
        }
        moneyReward.GetComponentInChildren<Text>().text = "X" + totalMoneyCount.ToString();
        moneyReward.GetComponent<Button>().onClick.AddListener(() => GetDropMoney(moneyReward, totalMoneyCount));
        if (MapManager.Instance.MapNodes[count][id].l.LevelType == "FINALBOSS")
        {
            int randomIndex = Random.Range(2001, 2006);
            GameObject skillReward = Instantiate(rewardPrefab, rewardGroupTrans);
            skillReward.GetComponent<Image>().sprite = DataManager.Instance.SkillList[randomIndex].SkillSprite;
            skillReward.GetComponent<Button>().onClick.AddListener(() => GetTalentSkillReward(skillReward, randomIndex));
        }
    }
}
