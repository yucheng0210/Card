using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleManager : Singleton<BattleManager>
{
    public enum BattleType
    {
        None,
        Explore,
        BattleInitial,
        Dialog,
        Player,
        Attack,
        UsingEffect,
        Enemy,
        Win,
        Victory,
        Loss
    }
    public enum NegativeState
    {
        CantMove
    }
    //玩家
    private int playerMoveCount;
    public int PlayerMoveCount
    {
        get { return playerMoveCount; }
        set
        {
            playerMoveCount = value;
            if (playerMoveCount > 2)
                playerMoveCount = 2;
        }
    }
    public BattleType MyBattleType { get; set; }
    public List<NegativeState> CurrentNegativeState { get; set; }
    public List<CardItem> CardItemList { get; set; }
    public List<Vector2> CardPositionList { get; set; }
    public List<float> CardAngleList { get; set; }
    public Dictionary<string, int> CurrentAbilityList { get; set; }
    public Dictionary<string, string> CurrentTrapList { get; set; }
    public List<string> StateEventList { get; set; }
    //敵人
    public Dictionary<string, EnemyData> CurrentEnemyList { get; set; }
    public bool IsDrag { get; set; }
    //棋盤
    public string CurrentLocationID { get; set; }
    public Dictionary<string, Terrain> CurrentTerrainList { get; set; }
    public Dictionary<string, string> CheckerboardList { get; set; }
    public RectTransform PlayerTrans { get; set; }
    public RectTransform CheckerboardTrans { get; set; }

    protected override void Awake()
    {
        base.Awake();
        IsDrag = false;
        CardItemList = new List<CardItem>();
        CardPositionList = new List<Vector2>();
        CardAngleList = new List<float>();
        CurrentEnemyList = new Dictionary<string, EnemyData>();
        CurrentAbilityList = new Dictionary<string, int>();
        CheckerboardList = new Dictionary<string, string>();
        CurrentTerrainList = new Dictionary<string, Terrain>();
        CurrentNegativeState = new List<NegativeState>();
        CurrentTrapList = new Dictionary<string, string>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < CurrentEnemyList.Count; i++)
            {
                CharacterData value = CurrentEnemyList.ElementAt(i).Value;
                TakeDamage(value, 99, CurrentEnemyList.ElementAt(i).Key);
            }
        }
    }
    public void TakeDamage(CharacterData defender, int damage, string location)
    {
        int currentDamage = damage - defender.CurrentShield;
        if (currentDamage < 0)
            currentDamage = 0;
        defender.CurrentShield -= damage;
        defender.CurrentHealth -= currentDamage;
        int point = GetCheckerboardPoint(location);
        Vector2 pos = new(CheckerboardTrans.GetChild(point).localPosition.x, CheckerboardTrans.GetChild(point).localPosition.y);
        EventManager.Instance.DispatchEvent(EventDefinition.eventTakeDamage, pos, damage, location);
    }

    public void GetShield(CharacterData defender, int point)
    {
        defender.CurrentShield += point;
    }

    public void ConsumeActionPoint(int point)
    {
        if (DataManager.Instance.PlayerList[DataManager.Instance.PlayerID].CurrentActionPoint >= point)
            DataManager.Instance.PlayerList[DataManager.Instance.PlayerID].CurrentActionPoint -= point;
    }
    public int[] ConvertNormalPos(string location)
    {
        string[] myLocation = location.Split(' ');
        int[] normalPos = new int[2];
        normalPos[0] = int.Parse(myLocation[0]);
        normalPos[1] = int.Parse(myLocation[1]);
        return normalPos;
    }
    public string ConvertCheckerboardPos(int x, int y)
    {
        return x.ToString() + ' ' + y.ToString();
    }
    public int GetCheckerboardPoint(string point)
    {
        string[] points = point.Split(' ');
        return int.Parse(points[0]) + int.Parse(points[1]) * 8;
    }
    public string ConvertCheckerboardPos(int point)
    {
        int x = point % 8;
        int y = point / 8;
        return x.ToString() + ' ' + y.ToString();
    }
    public List<string> GetEmptyPlace(string location, int stepCount, CheckEmptyType checkEmptyType)
    {
        List<string> emptyPlaceList = new();
        int[] pos = ConvertNormalPos(location);
        Vector2Int point = new Vector2Int(pos[0], pos[1]);
        int minX = point.x - stepCount;
        int maxX = point.x + stepCount;
        int minY = point.y - stepCount;
        int maxY = point.y + stepCount;

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                // 跳過起始點
                if (x == point.x && y == point.y)
                    continue;

                bool isInnerCircle = x > minX && x < maxX && y > minY && y < maxY;
                bool isBoundary = (x == minX && y == point.y) || (x == maxX && y == point.y) || (y == minY && x == point.x) || (y == maxY && x == point.x);
                if (isInnerCircle || isBoundary)
                {
                    string targetPos = ConvertCheckerboardPos(x, y);
                    if (CheckPlaceEmpty(targetPos, checkEmptyType) && CheckUnBlock(location, targetPos))
                        emptyPlaceList.Add(targetPos);
                }
            }
        }

        return emptyPlaceList;
    }
    public enum CheckEmptyType
    {
        PlayerAttack,
        EnemyAttack,
        Move
    }
    private bool CheckPlaceEmpty(string place, CheckEmptyType checkEmptyType)
    {
        if (!CheckerboardList.ContainsKey(place))
            return false;
        string placeStatus = CheckerboardList[place];
        if ((checkEmptyType == CheckEmptyType.PlayerAttack && placeStatus == "Enemy") || (checkEmptyType == CheckEmptyType.EnemyAttack && placeStatus == "Player") || placeStatus == "Empty")
            return true;
        return false;
    }
    public bool CheckUnBlock(string fromLocation, string toLocation)
    {
        int[] fromPos = ConvertNormalPos(fromLocation);
        int[] toPos = ConvertNormalPos(toLocation);
        Vector2Int from = new Vector2Int(fromPos[0], fromPos[1]);
        Vector2Int to = new Vector2Int(toPos[0], toPos[1]);

        int steps = Mathf.Max(Mathf.Abs(to.x - from.x), Mathf.Abs(to.y - from.y));
        for (int i = 1; i < steps; i++)
        {
            float t = (float)i / steps;
            int x = Mathf.RoundToInt(Mathf.Lerp(from.x, to.x, t));
            int y = Mathf.RoundToInt(Mathf.Lerp(from.y, to.y, t));
            string intermediatePos = ConvertCheckerboardPos(x, y);

            if (CheckerboardList.ContainsKey(intermediatePos) && CheckerboardList[intermediatePos] != "Empty")
                return false;
        }

        return true;
    }
    public float GetDistance(string location)
    {
        int[] playerNormalPos = ConvertNormalPos(CurrentLocationID);
        int[] enemyNormalPos = ConvertNormalPos(location);
        float distance = Mathf.Sqrt(Mathf.Pow(playerNormalPos[0] - enemyNormalPos[0], 2) + Mathf.Pow(playerNormalPos[1] - enemyNormalPos[1], 2));
        return distance;
    }

    public void RefreshCheckerboardList()
    {
        CheckerboardList.Clear();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                string location = ConvertCheckerboardPos(i, j);
                if (CurrentLocationID == location)
                {
                    CheckerboardList.Add(location, "Player");
                    //Debug.Log("玩家：" + location);
                }
                else if (CurrentEnemyList.ContainsKey(location))
                {
                    CheckerboardList.Add(location, "Enemy");
                    // Debug.Log("敵人：" + location);
                }
                else if (CurrentTerrainList.ContainsKey(location))
                    CheckerboardList.Add(location, "Terrain");
                else
                    CheckerboardList.Add(location, "Empty");
            }
        }
    }
    public void ConsumeMana(int consumeMana)
    {
        DataManager.Instance.PlayerList[DataManager.Instance.PlayerID].Mana -= consumeMana;
    }
    public void ChangeTurn(BattleType type)
    {
        MyBattleType = type;
        switch (MyBattleType)
        {
            case BattleType.None:
                //UIManager.Instance.HideUI("UIBattle");
                break;
            case BattleType.Explore:
                Explore();
                break;
            case BattleType.BattleInitial:
                BattleInitial();
                break;
            case BattleType.Attack:
                Attack();
                break;
            case BattleType.Dialog:
                Dialog();
                break;
            case BattleType.Player:
                PlayerTurn();
                break;
            case BattleType.Enemy:
                EnemyTurn();
                break;
            case BattleType.Win:
                Win();
                break;
            case BattleType.Loss:
                break;
        }
    }
    private void BattleInitial()
    {
        int playerID = DataManager.Instance.PlayerID;
        int levelID = MapManager.Instance.LevelID;
        int skillID = DataManager.Instance.PlayerList[playerID].StartSkill;
        int levelCount = MapManager.Instance.LevelCount;
        CurrentLocationID = MapManager.Instance.MapNodes[levelCount][levelID].l.PlayerStartPos;
        DataManager.Instance.PlayerList[playerID].CurrentActionPoint = DataManager.Instance.PlayerList[playerID].MaxActionPoint;
        DataManager.Instance.PlayerList[playerID].Mana = 10;
        PlayerTrans.localPosition = CheckerboardTrans.GetChild(GetCheckerboardPoint(CurrentLocationID)).localPosition;
        for (int i = 0; i < DataManager.Instance.SkillList[skillID].SkillContent.Count; i++)
        {
            CurrentAbilityList.Add(DataManager.Instance.SkillList[skillID].SkillContent[i].Item1, DataManager.Instance.SkillList[skillID].SkillContent[i].Item2);
        }
        for (int i = 0; i < MapManager.Instance.MapNodes[levelCount][levelID].l.EnemyIDList.Count; i++)
        {
            int enemyID = MapManager.Instance.MapNodes[levelCount][levelID].l.EnemyIDList.ElementAt(i).Value;
            string loactionID = MapManager.Instance.MapNodes[levelCount][levelID].l.EnemyIDList.ElementAt(i).Key;
            CurrentEnemyList.Add(loactionID, (EnemyData)DataManager.Instance.EnemyList[enemyID].Clone());
        }
        for (int i = 0; i < MapManager.Instance.MapNodes[levelCount][levelID].l.TerrainIDList.Count; i++)
        {
            int terrainID = MapManager.Instance.MapNodes[levelCount][levelID].l.TerrainIDList.ElementAt(i).Value;
            string locationID = MapManager.Instance.MapNodes[levelCount][levelID].l.TerrainIDList.ElementAt(i).Key;
            CurrentTerrainList.Add(locationID, DataManager.Instance.TerrainList[terrainID].Clone());
        }
        EventManager.Instance.DispatchEvent(EventDefinition.eventBattleInitial);
    }
    private void Attack()
    {
        //StartCoroutine(UIManager.Instance.RefreshEnemyAlert());
        RefreshCheckerboardList();
    }
    private void Explore()
    {
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
        EventManager.Instance.DispatchEvent(
            EventDefinition.eventExplore,
            MapManager.Instance.MapNodes[MapManager.Instance.LevelCount][MapManager.Instance.LevelID].l.LevelType
        );
    }

    private void Dialog()
    {
        EventManager.Instance.DispatchEvent(EventDefinition.eventDialog);
    }

    private void PlayerTurn()
    {
        int playerID = DataManager.Instance.PlayerID;
        DataManager.Instance.PlayerList[playerID].CurrentActionPoint = DataManager.Instance.PlayerList[playerID].MaxActionPoint;
        //DataManager.Instance.PlayerList[playerID].Mana++;
        DataManager.Instance.PlayerList[playerID].CurrentShield = 0;
        for (int i = 0; i < CurrentAbilityList.Count; i++)
        {
            string effectID;
            int effectCount;
            effectID = CurrentAbilityList.ElementAt(i).Key;
            effectCount = CurrentAbilityList.ElementAt(i).Value;
            EffectFactory.Instance.CreateEffect(effectID).ApplyEffect(effectCount, "Player");

        }
        EventManager.Instance.DispatchEvent(EventDefinition.eventPlayerTurn);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    private void EnemyTurn()
    {
        EventManager.Instance.DispatchEvent(EventDefinition.eventEnemyTurn);
        CurrentNegativeState.Clear();
    }

    private void Win()
    {
        EventManager.Instance.DispatchEvent(EventDefinition.eventBattleWin);
    }

    public void Shuffle()
    {
        for (int i = 0; i < DataManager.Instance.CardBag.Count; i++)
        {
            int randomIndex = Random.Range(0, DataManager.Instance.CardBag.Count);
            CardData temp = DataManager.Instance.CardBag[randomIndex];
            DataManager.Instance.CardBag[randomIndex] = DataManager.Instance.CardBag[i];
            DataManager.Instance.CardBag[i] = temp;
        }
    }
    public void NextLevel(string hideMenu)
    {
        // BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Dialog);
        MapManager.Instance.LevelCount++;
        UIManager.Instance.ShowUI("UIMap");
        UIManager.Instance.HideUI(hideMenu);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }
}
