using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;

public class DataManager : Singleton<DataManager>, ISavable
{
    private readonly string cardListPath = Application.streamingAssetsPath + "/CARDLIST.csv";
    private readonly string playerListPath = Application.streamingAssetsPath + "/PLAYERLIST.csv";
    private readonly string enemyListPath = Application.streamingAssetsPath + "/ENEMYLIST.csv";
    private readonly string levelTypeListPath = Application.streamingAssetsPath + "/LEVELTYPELIST.csv";
    private readonly string itemListPath = Application.streamingAssetsPath + "/ITEMLIST.csv";
    private readonly string potionListPath = Application.streamingAssetsPath + "/POTIONLIST.csv";
    private readonly string dialogDataListPath = Application.streamingAssetsPath + "/DialogData";
    private readonly string terrainListPath = Application.streamingAssetsPath + "/TERRAINLIST.csv";
    private readonly string skillListPath = Application.streamingAssetsPath + "/SKILLLIST.csv";
    private readonly string trapListPath = Application.streamingAssetsPath + "/TRAPLIST.csv";
    public Dictionary<int, CardData> CardList { get; set; }
    public List<CardData> CardBag { get; set; }
    public List<CardData> UsedCardBag { get; set; }
    public List<CardData> RemoveCardBag { get; set; }
    public List<CardData> HandCard { get; set; }
    public List<Potion> PotionBag { get; set; }
    public Dictionary<int, PlayerData> PlayerList { get; set; }
    public Dictionary<int, EnemyData> EnemyList { get; set; }
    public Dictionary<int, Level> LevelList { get; set; }
    public Dictionary<int, Level> LevelTypeList { get; set; }
    public Dictionary<int, Item> ItemList { get; set; }
    public Dictionary<int, Potion> PotionList { get; set; }
    public Dictionary<int, Item> Backpack { get; set; }
    public Dictionary<int, Item> ShopBag { get; set; }
    public Dictionary<int, Skill> SkillList { get; set; }
    public Dictionary<int, TrapData> TrapList { get; set; }
    public Dictionary<string, List<Dialog>> DialogList { get; set; }
    public Dictionary<int, Terrain> TerrainList { get; set; }
    public int PlayerID { get; set; }
    public int MoneyCount { get; set; }
    private float gameTime;
    protected override void Awake()
    {
        base.Awake();
        CardList = new Dictionary<int, CardData>();
        CardBag = new List<CardData>();
        UsedCardBag = new List<CardData>();
        RemoveCardBag = new List<CardData>();
        PotionBag = new List<Potion>();
        HandCard = new List<CardData>();
        PlayerList = new Dictionary<int, PlayerData>();
        EnemyList = new Dictionary<int, EnemyData>();
        LevelList = new Dictionary<int, Level>();
        LevelTypeList = new Dictionary<int, Level>();
        ItemList = new Dictionary<int, Item>();
        PotionList = new Dictionary<int, Potion>();
        Backpack = new Dictionary<int, Item>();
        ShopBag = new Dictionary<int, Item>();
        DialogList = new Dictionary<string, List<Dialog>>();
        TerrainList = new Dictionary<int, Terrain>();
        SkillList = new Dictionary<int, Skill>();
        TrapList = new Dictionary<int, TrapData>();
        LoadData();
    }

    private void Start()
    {
        //StartGame_Default();
        //StartGame_FightingSpiritEffect();
        StartGame_ExtinctionRayEffect();
        AddSavableRegister();
    }
    private void Update()
    {
        gameTime += Time.unscaledDeltaTime;
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
                AutoCardRemove = bool.Parse(row[17]),
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
                StartSkillList = row[6].Split(";").Select(int.Parse).ToList(),
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
                AttackRange = int.Parse(row[7]),
                MeleeAttackMode = bool.Parse(row[8]),
                AttackOrderStrs = new List<(string, int)>(),
                EnemyAniPath = row[10],
                PassiveSkills = new Dictionary<string, int>(),
                MaxPassiveSkillsList = new Dictionary<string, int>(),
                DamageReduction = int.Parse(row[12]),
                ImageFlip = bool.Parse(row[13]),
                SpecialAttackCondition = int.Parse(row[14]),
                SpecialAttackOrderStrs = new List<(string, int)>(),
                SpecialMechanismList = new Dictionary<string, int>(),
                SpecialTriggerSkill = new(),
                CurrentAttackOrderStrs = new(),
                IsMinion = bool.Parse(row[18]),
                DropMoney = int.Parse(row[19]),
                AttackParticleEffectPath = row[20],
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
                    if (skillParts.Length == 2)
                    {
                        enemyData.MaxPassiveSkillsList.Add(skillParts[0], int.Parse(skillParts[1]));
                        enemyData.PassiveSkills.Add(skillParts[0], int.Parse(skillParts[1]));
                    }
                }
            }
            if (!string.IsNullOrEmpty(row[15]))
            {
                string[] specialAttackOrders = row[15].Split(';');
                for (int j = 0; j < specialAttackOrders.Length; j++)
                {
                    string[] orderParts = specialAttackOrders[j].Split('=');
                    if (orderParts.Length == 2)
                    {
                        enemyData.SpecialAttackOrderStrs.Add((orderParts[0], int.Parse(orderParts[1])));
                    }
                }
            }
            if (!string.IsNullOrEmpty(row[16]))
            {
                string[] specialMechanismList = row[16].Split(';');
                for (int j = 0; j < specialMechanismList.Length; j++)
                {
                    string[] orderParts = specialMechanismList[j].Split('=');
                    if (orderParts.Length == 2)
                    {
                        enemyData.SpecialMechanismList.Add(orderParts[0], int.Parse(orderParts[1]));
                    }
                }
            }
            if (!string.IsNullOrEmpty(row[17]))
            {
                string[] specialTriggerSkill = row[17].Split('=');
                if (specialTriggerSkill.Length == 2)
                {
                    enemyData.SpecialTriggerSkill = (specialTriggerSkill[0], int.Parse(specialTriggerSkill[1]));
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
        #region 藥水列表
        lineData = File.ReadAllLines(potionListPath);
        for (int i = 1; i < lineData.Length; i++)
        {
            string[] row = lineData[i].Split(',');
            Potion item = new()
            {
                ItemID = int.Parse(row[0]),
                ItemName = row[1],
                ItemImagePath = row[2],
                ItemInfo = row[3],
                ItemBuyPrice = int.Parse(row[4]),
                ItemSellPrice = int.Parse(row[5]),
                ItemEffectName = row[6],
                ItemRarity = row[7],
                ItemType = row[8],
                SynthesisItemList = new List<Item>()
            };
            if (!string.IsNullOrEmpty(row[9]))
            {
                string[] potions = row[9].Split(';');
                for (int j = 0; j < potions.Length; j++)
                {
                    Item synthesisItem = ItemList[int.Parse(potions[j])];
                    item.SynthesisItemList.Add(synthesisItem);
                }
            }
            PotionList.Add(item.ItemID, item);
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
                SkillContent = new Dictionary<string, int>(),
                SkillType = row[4],
                SkillSprite = Resources.Load<Sprite>(row[5]),
                IsTalentSkill = bool.Parse(row[6]),
                TalentAnimatorController = Resources.Load<AnimatorOverrideController>(row[7]),
            };
            if (row[3] != "")
            {
                string[] skillEffects = row[3].Split(';');
                for (int j = 0; j < skillEffects.Length; j++)
                {
                    string[] skillEffect = skillEffects[j].Split('=');
                    skill.SkillContent.Add(skillEffect[0], int.Parse(skillEffect[1]));
                }
            }
            SkillList.Add(skill.SkillID, skill);
        }
        #endregion
        #region 陷阱列表
        lineData = File.ReadAllLines(trapListPath);
        for (int i = 1; i < lineData.Length; i++)
        {
            string[] row = lineData[i].Split(',');
            TrapData trapData = new()
            {
                TrapID = int.Parse(row[0]),
                TrapName = row[1],
                MaxHealth = int.Parse(row[2]),
                BaseAttack = int.Parse(row[3]),
                TriggerSkillList = new(),
                TrapImagePath = row[5]

            };
            if (!string.IsNullOrEmpty(row[4]))
            {
                string[] trapSkills = row[4].Split(';');
                for (int j = 0; j < trapSkills.Length; j++)
                {
                    string[] skillParts = trapSkills[j].Split('=');
                    if (skillParts.Length == 2)
                    {
                        trapData.TriggerSkillList.Add(skillParts[0], int.Parse(skillParts[1]));
                    }
                }
            }
            TrapList.Add(trapData.TrapID, trapData);
        }
        #endregion
    }
    private void StartGame_Default()
    {
        MoneyCount = 99;
        PlayerID = 1001;

        // Add six 1001 cards
        for (int i = 0; i < 5; i++)
        {
            CardBag.Add(CardList[1001].DeepClone());
        }

        // Add four 1002 cards
        for (int i = 0; i < 3; i++)
        {
            CardBag.Add(CardList[1002].DeepClone());
        }
        for (int i = 0; i < 2; i++)
        {
            CardBag.Add(CardList[1004].DeepClone());
        }
        // Add one 1003 card
        CardBag.Add(CardList[1003].DeepClone());

        // Add items to backpack and potion bag
        //BackpackManager.Instance.AddItem(3001, Backpack);
        PotionBag.Add(PotionList[1002]);

        // Set current player data
        BattleManager.Instance.CurrentPlayerData = PlayerList[PlayerID];
    }

    private void StartGame_FightingSpiritEffect()
    {
        MoneyCount = 180;
        PlayerID = 1001;

        // Add ten 1006 cards
        for (int i = 0; i < 10; i++)
        {
            CardBag.Add(CardList[1005].DeepClone());
        }

        // Add items to backpack and potion bag
        /* BackpackManager.Instance.AddItem(1001, Backpack);
         BackpackManager.Instance.AddItem(1002, Backpack);
         BackpackManager.Instance.AddItem(1003, Backpack);*/
        PotionBag.Add(PotionList[1001]);

        // Set current player data
        BattleManager.Instance.CurrentPlayerData = PlayerList[PlayerID];
    }

    private void StartGame_ExtinctionRayEffect()
    {
        MoneyCount = 99;
        PlayerID = 1001;

        // Add specific cards
        int[] cardIds = { 3001, 2002, 2003, 2004, 1006, 1010, 1009, 1008 };

        for (int i = 0; i < cardIds.Length; i++)
        {
            CardBag.Add(CardList[cardIds[i]].DeepClone());
        }
        PotionBag.Add(PotionList[1001]);
        PotionBag.Add(PotionList[1002]);
        // Set current player data
        BattleManager.Instance.CurrentPlayerData = PlayerList[PlayerID];
    }

    public void GenerateGameData(GameSaveData gameSaveData)
    {
        List<CardData> handCard = HandCard;
        List<CardData> useCardBag = UsedCardBag;
        List<CardData> removeCardBag = RemoveCardBag;
        List<CardData> cardBag = CardBag;
        for (int i = 0; i < handCard.Count; i++)
        {
            if (handCard[i].CardType == "詛咒")
            {
                continue;
            }
            cardBag.Add(handCard[i]);
        }
        for (int j = 0; j < useCardBag.Count; j++)
        {
            if (useCardBag[j].CardType == "詛咒")
            {
                continue;
            }
            cardBag.Add(useCardBag[j]);
        }
        for (int j = 0; j < removeCardBag.Count; j++)
        {
            if (removeCardBag[j].CardType == "詛咒")
            {
                continue;
            }
            cardBag.Add(removeCardBag[j]);
        }
        gameSaveData.DataName = $"第{MapManager.Instance.ChapterCount}章";
        if (MapManager.Instance.LevelCount > 14)
        {
            gameSaveData.DataName = "天空島";
        }
        gameSaveData.Backpack = Backpack;
        gameSaveData.CardBag = CardBag;
        gameSaveData.PotionBag = PotionBag;
        gameSaveData.MoneyCount = MoneyCount;
        gameSaveData.GameTime = gameTime;
        gameSaveData.CurrentScene = "Level1";
        gameSaveData.StartSkillList = BattleManager.Instance.CurrentPlayerData.StartSkillList;
    }

    public void RestoreGameData(GameSaveData gameSaveData)
    {
        Backpack = gameSaveData.Backpack;
        MoneyCount = gameSaveData.MoneyCount;
        gameTime = gameSaveData.GameTime;
        CardBag = gameSaveData.CardBag;
        PotionBag = gameSaveData.PotionBag;
        BattleManager.Instance.CurrentPlayerData.StartSkillList = gameSaveData.StartSkillList;
        StartCoroutine(SceneController.Instance.Transition(gameSaveData.CurrentScene));
    }

    public void AddSavableRegister()
    {
        SaveLoadManager.Instance.AddRegister(this);
    }
}
