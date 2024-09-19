using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;

public class DataManager : Singleton<DataManager>
{
    private readonly string cardListPath = Application.streamingAssetsPath + "/CARDLIST.csv";
    private readonly string playerListPath = Application.streamingAssetsPath + "/PLAYERLIST.csv";
    private readonly string enemyListPath = Application.streamingAssetsPath + "/ENEMYLIST.csv";
    private readonly string levelTypeListPath = Application.streamingAssetsPath + "/LEVELTYPELIST.csv";
    private readonly string itemListPath = Application.streamingAssetsPath + "/ITEMLIST.csv";
    private readonly string dialogDataListPath = Application.streamingAssetsPath + "/DialogData";
    private readonly string terrainListPath = Application.streamingAssetsPath + "/TERRAINLIST.csv";
    private readonly string skillListPath = Application.streamingAssetsPath + "/SKILLLIST.csv";
    public Dictionary<int, CardData> CardList { get; set; }
    public List<CardData> CardBag { get; set; }
    public List<CardItem> UsedCardBag { get; set; }
    public List<CardItem> RemoveCardBag { get; set; }
    public List<Item> PotionBag { get; set; }
    public List<CardItem> HandCard { get; set; }
    public Dictionary<int, PlayerData> PlayerList { get; set; }
    public Dictionary<int, EnemyData> EnemyList { get; set; }
    public Dictionary<int, Level> LevelList { get; set; }
    public Dictionary<int, Level> LevelTypeList { get; set; }
    public Dictionary<int, Item> ItemList { get; set; }
    public Dictionary<int, Item> Backpack { get; set; }
    public Dictionary<int, Item> ShopBag { get; set; }
    public Dictionary<int, Skill> SkillList { get; set; }
    public Dictionary<string, List<Dialog>> DialogList { get; set; }
    public Dictionary<int, Terrain> TerrainList { get; set; }
    public int PlayerID { get; set; }
    public int MoneyCount { get; set; }

    protected override void Awake()
    {
        base.Awake();
        CardList = new Dictionary<int, CardData>();
        CardBag = new List<CardData>();
        UsedCardBag = new List<CardItem>();
        RemoveCardBag = new List<CardItem>();
        PotionBag = new List<Item>();
        HandCard = new List<CardItem>();
        PlayerList = new Dictionary<int, PlayerData>();
        EnemyList = new Dictionary<int, EnemyData>();
        LevelList = new Dictionary<int, Level>();
        LevelTypeList = new Dictionary<int, Level>();
        ItemList = new Dictionary<int, Item>();
        Backpack = new Dictionary<int, Item>();
        ShopBag = new Dictionary<int, Item>();
        DialogList = new Dictionary<string, List<Dialog>>();
        TerrainList = new Dictionary<int, Terrain>();
        SkillList = new Dictionary<int, Skill>();
        LoadData();
    }

    private void Start()
    {
        //StartGame();
        StartGame_FightingSpiritEffect();
        //StartGame_ExtinctionRayEffect();
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
                CardRarity = row[11],
                CardAttackDistance = int.Parse(row[12]),
                CardManaCost = int.Parse(row[13]),
                CardRemove = bool.Parse(row[14]),
                CardBuyPrice = int.Parse(row[15]),
                CardFreeze = bool.Parse(row[16]),
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
                DefaultDrawCardCount = int.Parse(row[5]),
                StartSkill = int.Parse(row[6])
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
                StepCount = int.Parse(row[6]),
                AttackDistance = int.Parse(row[7]),
                AlertDistance = int.Parse(row[8]),
                AttackOrderStrs = new List<(string, int)>(),
                EnemyAniPath = row[10],
                PassiveSkills = new Dictionary<string, int>(),
                MaxPassiveSkillsList = new List<string>(),
                DamageReduction = int.Parse(row[12]),
                ImageFlip = bool.Parse(row[13]),
            };
            if (!string.IsNullOrEmpty(row[9]))
            {
                string[] attackOrders = row[9].Split(';');
                for (int j = 0; j < attackOrders.Length; j++)
                {
                    string[] orderParts = attackOrders[j].Split('=');
                    if (orderParts.Length == 2)
                    {
                        enemyData.AttackOrderStrs.Add((orderParts[0], int.Parse(orderParts[1])));
                    }
                }
            }
            if (!string.IsNullOrEmpty(row[11]))
            {
                string[] passiveSkills = row[11].Split(';');
                for (int j = 0; j < passiveSkills.Length; j++)
                {
                    string[] skillParts = passiveSkills[j].Split('=');
                    enemyData.MaxPassiveSkillsList.Add(skillParts[0]);
                    if (skillParts.Length == 2)
                    {
                        enemyData.PassiveSkills.Add(skillParts[0], int.Parse(skillParts[1]));
                    }
                }
            }
            EnemyList.Add(enemyData.CharacterID, enemyData);
        }

        #endregion
        #region 關卡列表
        /* lineData = File.ReadAllLines(levelListPath);
         for (int i = 1; i < lineData.Length; i++)
         {
             string[] row = lineData[i].Split(',');
             Level level = new()
             {
                 LevelID = int.Parse(row[0]),
                 LevelName = row[1],
                 DialogName = row[4],
                 LevelType = row[5],
                 PlayerStartPos = row[6],
                 LevelPassed = false
             };
             if (!string.IsNullOrEmpty(row[2]))
             {
                 string[] enemyIDs = row[2].Split(';');
                 level.EnemyIDList = new Dictionary<string, int>();
                 for (int j = 0; j < enemyIDs.Length; j++)
                 {
                     string[] enemyID = enemyIDs[j].Split('=');
                     level.EnemyIDList.Add(enemyID[0], int.Parse(enemyID[1]));
                 }
             }
             if (!string.IsNullOrEmpty(row[3]))
             {
                 string[] rewardIDs = row[3].Split(';');
                 level.RewardIDList = new List<(int, int)>();
                 for (int j = 0; j < rewardIDs.Length; j++)
                 {
                     string[] rewardID = rewardIDs[j].Split('=');
                     if (int.TryParse(rewardID[0], out int id) && int.TryParse(rewardID[1], out int count))
                         level.RewardIDList.Add(new ValueTuple<int, int>(id, count));
                 }
             }
             level.LevelParentList = new();
             if (!string.IsNullOrEmpty(row[7]))
             {
                 string[] parentIDs = row[7].Split(';');
                 for (int j = 0; j < parentIDs.Length; j++)
                 {
                     level.LevelParentList.Add(int.Parse(parentIDs[j]));
                 }
             }
             if (!string.IsNullOrEmpty(row[8]))
             {
                 string[] terrainIDs = row[8].Split(';');
                 level.TerrainIDList = new Dictionary<string, int>();
                 for (int j = 0; j < terrainIDs.Length; j++)
                 {
                     string[] terrainID = terrainIDs[j].Split('=');
                     level.TerrainIDList.Add(terrainID[0], int.Parse(terrainID[1]));
                 }
             }
             LevelList.Add(level.LevelID, level);
         }*/
        #endregion
        #region 關卡類型列表
        lineData = File.ReadAllLines(levelTypeListPath);
        for (int i = 1; i < lineData.Length; i++)
        {
            string[] row = lineData[i].Split(',');
            Level level = new()
            {
                LevelID = int.Parse(row[0]),
                LevelName = row[1],
                //DialogName = row[4],
                LevelType = row[4],
                PlayerStartPos = row[5],
                LevelActive = false
            };
            if (!string.IsNullOrEmpty(row[2]))
            {
                string[] enemyIDs = row[2].Split(';');
                level.EnemyIDList = new Dictionary<string, int>();
                for (int j = 0; j < enemyIDs.Length; j++)
                {
                    string[] enemyID = enemyIDs[j].Split('=');
                    level.EnemyIDList.Add(enemyID[0], int.Parse(enemyID[1]));
                }
            }
            level.RewardIDList = new List<(int, int)>();
            if (!string.IsNullOrEmpty(row[3]))
            {
                string[] rewardIDs = row[3].Split(';');
                for (int j = 0; j < rewardIDs.Length; j++)
                {
                    string[] rewardID = rewardIDs[j].Split('=');
                    if (int.TryParse(rewardID[0], out int id) && int.TryParse(rewardID[1], out int count))
                        level.RewardIDList.Add(new ValueTuple<int, int>(id, count));
                }
            }
            /* level.LevelParentList = new();
             if (!string.IsNullOrEmpty(row[7]))
             {
                 string[] parentIDs = row[7].Split(';');
                 for (int j = 0; j < parentIDs.Length; j++)
                 {
                     level.LevelParentList.Add(int.Parse(parentIDs[j]));
                 }
             }*/
            if (!string.IsNullOrEmpty(row[6]))
            {
                string[] terrainIDs = row[6].Split(';');
                level.TerrainIDList = new Dictionary<string, int>();
                for (int j = 0; j < terrainIDs.Length; j++)
                {
                    string[] terrainID = terrainIDs[j].Split('=');
                    level.TerrainIDList.Add(terrainID[0], int.Parse(terrainID[1]));
                }
            }
            LevelTypeList.Add(level.LevelID, level);
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
        #region 地形列表
        lineData = File.ReadAllLines(terrainListPath);
        for (int i = 1; i < lineData.Length; i++)
        {
            string[] row = lineData[i].Split(',');
            Terrain terrain = new()
            {
                TerrainID = int.Parse(row[0]),
                TerrainName = row[1],
                MaxHealth = int.Parse(row[2]),
                MinAttack = int.Parse(row[3]),
                MaxAttack = int.Parse(row[4]),
                AttackDistance = int.Parse(row[5]),
                ImagePath = row[6]
            };
            TerrainList.Add(terrain.TerrainID, terrain);
        }
        #endregion
        #region 權能列表 
        lineData = File.ReadAllLines(skillListPath);
        for (int i = 1; i < lineData.Length; i++)
        {
            string[] row = lineData[i].Split(',');
            Skill skill = new()
            {
                SkillID = int.Parse(row[0]),
                SkillName = row[1],
                SkillDescrption = row[2],
                SkillContent = new List<(string, int)>(),
                SkillType = row[4]
            };
            if (row[3] != "")
            {
                string[] skillEffects = row[3].Split(';');
                for (int j = 0; j < skillEffects.Length; j++)
                {
                    string[] skillEffect = skillEffects[j].Split('=');
                    string id;
                    id = skillEffect[0];
                    if (int.TryParse(skillEffect[1], out int count))
                        skill.SkillContent.Add(new ValueTuple<string, int>(id, count));
                }
            }
            SkillList.Add(skill.SkillID, skill);
        }
        #endregion
    }
    private void StartGame()
    {
        MoneyCount = 99;
        PlayerID = 1001;
        CardData cardData;
        for (int i = 0; i < 6; i++)
        {
            cardData = CardList[1001];
            CardBag.Add(cardData);
        }
        for (int i = 0; i < 4; i++)
        {
            cardData = CardList[1002];
            CardBag.Add(cardData);
        }
        CardBag.Add(CardList[1003]);
        BackpackManager.Instance.AddItem(3001, Backpack);
        PotionBag.Add(ItemList[1001]);
        BattleManager.Instance.CurrentPlayerData = PlayerList[PlayerID];
        // PlayerList[PlayerID].CharacterPos = LevelList[LevelID].PlayerStartPos;
    }
    private void StartGame_FightingSpiritEffect()
    {
        MoneyCount = 99;
        PlayerID = 1001;
        CardData cardData;
        for (int i = 0; i < 10; i++)
        {
            cardData = CardList[1006];
            CardBag.Add(cardData);
        }
        BackpackManager.Instance.AddItem(3001, Backpack);
        PotionBag.Add(ItemList[1001]);
        // PlayerList[PlayerID].CharacterPos = LevelList[LevelID].PlayerStartPos;
        BattleManager.Instance.CurrentPlayerData = PlayerList[PlayerID];
    }
    private void StartGame_ExtinctionRayEffect()
    {
        MoneyCount = 99;
        PlayerID = 1001;
        CardData cardData;
        cardData = CardList[3001];
        CardBag.Add(cardData);
        cardData = CardList[2002];
        CardBag.Add(cardData);
        cardData = CardList[2003];
        CardBag.Add(cardData);
        cardData = CardList[2004];
        CardBag.Add(cardData);
        cardData = CardList[1011];
        CardBag.Add(cardData);
        cardData = CardList[1010];
        CardBag.Add(cardData);
        BattleManager.Instance.CurrentPlayerData = PlayerList[PlayerID];
    }
}
