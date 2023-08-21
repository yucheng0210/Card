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
    private const string dialogDataListPath = "Assets/MyAssets/Data/DialogData";
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
    public int MoneyCount { get; set; }

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
    }

    private void Start()
    {
        StartCoroutine(StartGame());
    }

    private void LoadData()
    {
        #region 卡牌列表
        string[] lineData = File.ReadAllLines(cardListPath);
        for (int i = 1; i < lineData.Length; i++)
        {
            string[] row = lineData[i].Split(',');
            CardData cardData = new()
            {
                CardID = int.Parse(row[0]),
                CardName = row[1],
                CardType = row[2],
                CardImagePath = row[3],
                CardCost = int.Parse(row[4]),
                CardAttribute = row[5],
                CardSpecialEffect = row[6],
                CardDescription = row[7],
                CardAttack = int.Parse(row[8]),
                CardShield = int.Parse(row[9]),
                CardEffectList = new List<(string, int)>()
            };
            if (row[10] != "")
            {
                string[] cardEffects = row[10].Split(';');
                for (int j = 0; j < cardEffects.Length; j++)
                {
                    string[] cardEffect = cardEffects[j].Split('=');
                    string id;
                    id = cardEffect[0];
                    if (int.TryParse(cardEffect[1], out int count))
                        cardData.CardEffectList.Add(new ValueTuple<string, int>(id, count));
                }
            }
            cardData.CardRarity = row[11];
            CardList.Add(cardData.CardID, cardData);
        }
        #endregion
        #region 角色列表
        lineData = File.ReadAllLines(playerListPath);
        for (int i = 1; i < lineData.Length; i++)
        {
            string[] row = lineData[i].Split(',');

            PlayerData playerData = new()
            {
                CharacterID = int.Parse(row[0]),
                CharacterName = row[1],
                MaxHealth = int.Parse(row[2]),
                MaxActionPoint = int.Parse(row[3]),
                Mana = int.Parse(row[4]),
                DefaultDrawCardCout = int.Parse(row[5]),
            };
            PlayerList.Add(playerData.CharacterID, playerData);
        }
        #endregion
        #region 敵人列表
        lineData = File.ReadAllLines(enemyListPath);
        for (int i = 1; i < lineData.Length; i++)
        {
            string[] row = lineData[i].Split(',');
            EnemyData enemyData = new()
            {
                CharacterID = int.Parse(row[0]),
                CharacterName = row[1],
                MaxHealth = int.Parse(row[2]),
                MinAttack = int.Parse(row[3]),
                MaxAttack = int.Parse(row[4]),
                EnemyImagePath = row[5],
                //CharacterPos = row[6]
            };
            EnemyList.Add(enemyData.CharacterID, enemyData);
        }

        #endregion
        #region 關卡列表
        lineData = File.ReadAllLines(levelListPath);
        for (int i = 1; i < lineData.Length; i++)
        {
            string[] row = lineData[i].Split(',');
            Level level = new()
            {
                LevelID = int.Parse(row[0]),
                LevelName = row[1]
            };
            string[] enemyIDs = row[2].Split(';');
            level.EnemyIDList = new Dictionary<string, int>();
            for (int j = 0; j < enemyIDs.Length; j++)
            {
                string[] enemyID = enemyIDs[j].Split('=');
                level.EnemyIDList.Add(enemyID[0], int.Parse(enemyID[1]));
            }
            string[] rewardIDs = row[3].Split(';');
            level.RewardIDList = new List<(int, int)>();
            for (int j = 0; j < rewardIDs.Length; j++)
            {
                string[] rewardID = rewardIDs[j].Split('=');
                if (int.TryParse(rewardID[0], out int id) && int.TryParse(rewardID[1], out int count))
                    level.RewardIDList.Add(new ValueTuple<int, int>(id, count));
            }
            level.DialogName = row[4];
            level.LevelType = row[5];
            level.PlayerStartPos = row[6];
            LevelList.Add(level.LevelID, level);
        }
        #endregion
        #region 物品列表
        lineData = File.ReadAllLines(itemListPath);
        for (int i = 1; i < lineData.Length; i++)
        {
            string[] row = lineData[i].Split(',');
            Item item = new()
            {
                ItemID = int.Parse(row[0]),
                ItemName = row[1],
                ItemImagePath = row[2],
                ItemInfo = row[3],
                ItemBuyPrice = int.Parse(row[4]),
                ItemSellPrice = int.Parse(row[5]),
                ItemEffectName = row[6],
                ItemRarity = row[7],
                ItemType = row[8]
            };
            ItemList.Add(item.ItemID, item);
        }
        #endregion
        #region 對話列表
        foreach (string file in Directory.GetFiles(dialogDataListPath, "*.csv"))
        {
            lineData = File.ReadAllLines(file);
            List<Dialog> dialogs = new();
            for (int i = 1; i < lineData.Length; i++)
            {
                string[] row = lineData[i].Split(',');
                Dialog dialog = new()
                {
                    Branch = row[0],
                    Type = row[1],
                    TheName = row[2],
                    Order = row[3],
                    Content = row[4]
                };
                dialogs.Add(dialog);
            }
            string fileName = Path.GetFileNameWithoutExtension(file);
            DialogList.Add(fileName, dialogs);
        }
        #endregion
    }

    public void LoadLevel() { }

    private IEnumerator StartGame()
    {
        MoneyCount = 0;
        PlayerID = 1001;
        LevelID = 1001;
        CardData cardData;
        for (int i = 0; i < 5; i++)
        {
            cardData = CardList[1001];
            CardBag.Add(cardData);
            cardData = CardList[1002];
            CardBag.Add(cardData);
        }
        CardBag.Add(CardList[1003]);
        CardBag.Add(CardList[1003]);
        CardBag.Add(CardList[1004]);
        CardBag.Add(CardList[1008]);
        PlayerList[PlayerID].CharacterPos = LevelList[LevelID].PlayerStartPos;
        yield return null;
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Explore);
    }
}
