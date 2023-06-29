using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class DataManager : Singleton<DataManager>
{
    private const string cardListPath = "Assets/MyAssets/Data/CARDLIST.csv";
    private const string playerListPath = "Assets/MyAssets/Data/PLAYERLIST.csv";
    private const string enemyListPath = "Assets/MyAssets/Data/ENEMYLIST.csv";
    private const string levelListPath = "Assets/MyAssets/Data/LEVELLIST.csv";
    private const string itemListPath = "Assets/MyAssets/Data/ITEMLIST.csv";
    private string dialogDataListPath = "Assets/MyAssets/Data/DialogData";
    public Dictionary<int, CardData> CardList { get; set; }
    public List<CardData> CardBag { get; set; }
    public List<CardItem> UsedCardBag { get; set; }
    public List<CardItem> HandCard { get; set; }
    public Dictionary<int, PlayerData> PlayerList { get; set; }
    public Dictionary<int, EnemyData> EnemyList { get; set; }
    public Dictionary<int, Level> LevelList { get; set; }
    public Dictionary<int, Item> ItemList { get; set; }
    public Dictionary<int, Item> Backpack { get; set; }
    public Dictionary<string, List<Dialog>> DialogList { get; set; }
    public int PlayerID { get; set; }
    public int LevelID { get; set; }

    protected override void Awake()
    {
        base.Awake();
        CardList = new Dictionary<int, CardData>();
        CardBag = new List<CardData>();
        HandCard = new List<CardItem>();
        PlayerList = new Dictionary<int, PlayerData>();
        EnemyList = new Dictionary<int, EnemyData>();
        LevelList = new Dictionary<int, Level>();
        UsedCardBag = new List<CardItem>();
        ItemList = new Dictionary<int, Item>();
        Backpack = new Dictionary<int, Item>();
        DialogList = new Dictionary<string, List<Dialog>>();
        LoadData();
        GameStart();
    }

    private void Start()
    {
        EventManager.Instance.AddEventRegister(EventDefinition.eventStartGame, GameStart);
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
            cardData.CardSpecialEffect = row[6];
            cardData.CardDescription = row[7];
            cardData.CardAttack = int.Parse(row[8]);
            cardData.CardShield = int.Parse(row[9]);
            cardData.CardEffectList = new List<(string, int)>();
            if (row[10] != "")
            {
                string[] cardEffects = row[10].Split(';');
                for (int j = 0; j < cardEffects.Length; j++)
                {
                    string[] cardEffect = cardEffects[j].Split('=');
                    string id;
                    int count;
                    id = cardEffect[0];
                    if (int.TryParse(cardEffect[1], out count))
                        cardData.CardEffectList.Add(new ValueTuple<string, int>(id, count));
                }
            }
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
            playerData.CurrentHealth = playerData.MaxActionPoint;
            playerData.CurrentActionPoint = playerData.MaxActionPoint;
            playerData.CharacterPos = new Vector2(-447f, 121f);
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
            enemyData.MinAttack = int.Parse(row[3]);
            enemyData.MaxAttack = int.Parse(row[4]);
            enemyData.CurrentHealth = enemyData.MaxHealth;
            enemyData.CharacterPos = new Vector2(474f, 121f);
            EnemyList.Add(enemyData.CharacterID, enemyData);
        }

        #endregion
        #region 關卡列表
        lineData = File.ReadAllLines(levelListPath);
        for (int i = 1; i < lineData.Length; i++)
        {
            string[] row = lineData[i].Split(',');
            Level level = new Level();
            level.LevelID = int.Parse(row[0]);
            level.LevelName = row[1];
            string[] enemyIDs = row[2].Split(';');
            for (int j = 0; j < enemyIDs.Length; j++)
            {
                string[] enemyID = enemyIDs[j].Split('=');
                int id,
                    count;
                level.EnemyIDList = new List<(int, int)>();
                if (int.TryParse(enemyID[0], out id) && int.TryParse(enemyID[1], out count))
                    level.EnemyIDList.Add(new ValueTuple<int, int>(id, count));
            }
            string[] rewardIDs = row[3].Split(';');
            for (int j = 0; j < rewardIDs.Length; j++)
            {
                string[] rewardID = rewardIDs[j].Split('=');
                int id,
                    count;
                level.RewardIDList = new List<(int, int)>();
                if (int.TryParse(rewardID[0], out id) && int.TryParse(rewardID[1], out count))
                    level.RewardIDList.Add(new ValueTuple<int, int>(id, count));
            }
            LevelList.Add(level.LevelID, level);
        }
        #endregion
        #region 物品列表
        lineData = File.ReadAllLines(itemListPath);
        for (int i = 1; i < lineData.Length; i++)
        {
            string[] row = lineData[i].Split(',');
            Item item = new Item();
            item.ItemID = int.Parse(row[0]);
            item.ItemName = row[1];
            item.ItemImagePath = row[2];
            item.ItemInfo = row[3];
            item.ItemBuyPrice = int.Parse(row[4]);
            item.ItemSellPrice = int.Parse(row[5]);
            item.ItemEffectName = row[6];
            item.ItemRarity = row[7];
            item.ItemType = row[8];
            ItemList.Add(item.ItemID, item);
        }
        #endregion
        #region 對話列表
        foreach (string file in Directory.GetFiles(dialogDataListPath, "*.csv"))
        {
            lineData = File.ReadAllLines(file);
            List<Dialog> dialogs = new List<Dialog>();
            for (int i = 1; i < lineData.Length; i++)
            {
                string[] row = lineData[i].Split(',');
                Dialog dialog = new Dialog();
                dialog.Branch = row[0];
                dialog.Type = row[1];
                dialog.TheName = row[2];
                dialog.Order = row[3];
                dialog.Content = row[4];
                dialogs.Add(dialog);
            }
            string fileName = Path.GetFileNameWithoutExtension(file);
            DialogList.Add(fileName, dialogs);
        }
        #endregion
    }

    public void LoadLevel() { }

    private void GameStart(params object[] args)
    {
        PlayerID = 1001;
        LevelID = 1001;
        CardData cardData = new CardData();
        for (int i = 0; i < 5; i++)
        {
            cardData = CardList[1001];
            CardBag.Add(cardData);
            cardData = CardList[1002];
            CardBag.Add(cardData);
        }
        cardData = CardList[1003];
        CardBag.Add(cardData);
        CardBag.Add(cardData);
        cardData = CardList[1004];
        CardBag.Add(cardData);
        DataManager.Instance.PlayerList[PlayerID].Mana = 0;
    }
}
