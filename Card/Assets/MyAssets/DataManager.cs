using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataManager : Singleton<DataManager>
{
    private const string cardListPath = "Assets/MyAssets/Data/CARDLIST.csv";
    private const string playerListPath = "Assets/MyAssets/Data/PLAYERLIST.csv";
    private const string enemyListPath = "Assets/MyAssets/Data/ENEMYLIST.csv";
    public Dictionary<int, CardData> CardList { get; set; }
    public List<CardData> CardBag { get; set; }
    public List<CardItem> HandCard { get; set; }
    public Dictionary<int, PlayerData> PlayerList { get; set; }
    public Dictionary<int, EnemyData> EnemyList { get; set; }

    protected override void Awake()
    {
        base.Awake();
        CardList = new Dictionary<int, CardData>();
        CardBag = new List<CardData>();
        HandCard = new List<CardItem>();
        PlayerList = new Dictionary<int, PlayerData>();
        EnemyList = new Dictionary<int, EnemyData>();
        LoadData();
        GameStart();
    }

    private void LoadData()
    {
        #region 卡牌列表
        string[] lineData = File.ReadAllLines(cardListPath);
        for (int i = 1; i < lineData.Length; i++)
        {
            string[] row = lineData[i].Split(',');
            CardData cardData = new CardData();
            cardData.CardID = int.Parse(row[0]);
            cardData.CardName = row[1];
            cardData.CardType = row[2];
            cardData.CardImagePath = row[3];
            cardData.CardCost = int.Parse(row[4]);
            cardData.CardAttribute = row[5];
            cardData.CardEffect = row[6];
            cardData.CardDescription = row[7];
            CardList.Add(cardData.CardID, cardData);
        }
        #endregion
        #region 角色列表
        lineData = File.ReadAllLines(playerListPath);
        for (int i = 1; i < lineData.Length; i++)
        {
            string[] row = lineData[i].Split(',');
            PlayerData playerData = new PlayerData();
            playerData.CharacterID = int.Parse(row[0]);
            playerData.CharacterName = row[1];
            playerData.MaxHealth = int.Parse(row[2]);
            playerData.MaxActionPoint = int.Parse(row[3]);
            PlayerList.Add(playerData.CharacterID, playerData);
        }
        #endregion
        #region 敵人列表
        lineData = File.ReadAllLines(enemyListPath);
        for (int i = 1; i < lineData.Length; i++)
        {
            string[] row = lineData[i].Split(',');
            EnemyData enemyData = new EnemyData();
            enemyData.CharacterID = int.Parse(row[0]);
            enemyData.CharacterName = row[1];
            enemyData.MaxHealth = int.Parse(row[2]);
            EnemyList.Add(enemyData.CharacterID, enemyData);
        }

        #endregion
    }

    private void GameStart()
    {
        CardData cardData = new CardData();
        for (int i = 0; i < 5; i++)
        {
            cardData = CardList[1001];
            CardBag.Add(cardData);
            cardData = CardList[2001];
            CardBag.Add(cardData);
        }
    }
}
