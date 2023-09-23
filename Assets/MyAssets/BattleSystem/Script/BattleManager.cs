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
    public BattleType MyBattleType { get; set; }
    public List<NegativeState> CurrentNegativeState { get; set; }
    public List<CardItem> CardItemList { get; set; }
    public List<Vector2> CardPositionList { get; set; }
    public List<float> CardAngleList { get; set; }
    public Dictionary<int, string> CurrentAbilityList { get; set; }
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
        CurrentAbilityList = new Dictionary<int, string>();
        CheckerboardList = new Dictionary<string, string>();
        CurrentTerrainList = new Dictionary<string, Terrain>();
        CurrentNegativeState = new List<NegativeState>();
    }
    public void TakeDamage(CharacterData defender, int damage, string loaction)
    {
        int currentDamage = damage - defender.CurrentShield;
        if (currentDamage < 0)
            currentDamage = 0;
        defender.CurrentShield -= damage;
        defender.CurrentHealth -= currentDamage;
        Vector2 pos = new(CheckerboardTrans.GetChild(GetCheckerboardPoint(loaction)).localPosition.x
        , CheckerboardTrans.GetChild(GetCheckerboardPoint(loaction)).localPosition.y);
        EventManager.Instance.DispatchEvent(EventDefinition.eventTakeDamage, pos, damage);
    }

    public void GetShield(CharacterData defender, int point)
    {
        defender.CurrentShield += point;
    }

    public void ConsumeActionPoint(int point)
    {
        if (
            DataManager.Instance.PlayerList[DataManager.Instance.PlayerID].CurrentActionPoint
            >= point
        )
            DataManager.Instance.PlayerList[DataManager.Instance.PlayerID].CurrentActionPoint -=
                point;
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
    public List<string> GetEmptyPlace(string location, int stepCount)
    {
        List<string> emptyPlaceList = new();
        int[] pos = ConvertNormalPos(location);
        int x = pos[0];
        int y = pos[1];
        string up = "", down = "", left = "", right = "", upRight = "", upLeft = "", downRight = "", downLeft = "";
        for (int i = 1; i <= stepCount; i++)
        {
            if (up != "CantMove")
                up = ConvertCheckerboardPos(x, y + i);
            if (down != "CantMove")
                down = ConvertCheckerboardPos(x, y - i);
            if (left != "CantMove")
                left = ConvertCheckerboardPos(x - i, y);
            if (right != "CantMove")
                right = ConvertCheckerboardPos(x + i, y);
            //上
            if (CheckerboardList.ContainsKey(up) && CheckerboardList[up] == "Empty")
                emptyPlaceList.Add(up);
            else
                up = "CantMove";
            //下
            if (CheckerboardList.ContainsKey(down) && CheckerboardList[down] == "Empty")
                emptyPlaceList.Add(down);
            else
                down = "CantMove";
            //左
            if (CheckerboardList.ContainsKey(left) && CheckerboardList[left] == "Empty")
                emptyPlaceList.Add(left);
            else
                left = "CantMove";
            //右
            if (CheckerboardList.ContainsKey(right) && CheckerboardList[right] == "Empty")
                emptyPlaceList.Add(right);
            else
                right = "CantMove";
            if (i == stepCount)
                break;
            if (upRight != "CantMove")
                upRight = ConvertCheckerboardPos(x + i, y + i);
            if (upLeft != "CantMove")
                upLeft = ConvertCheckerboardPos(x - i, y + i);
            if (downRight != "CantMove")
                downRight = ConvertCheckerboardPos(x + i, y - i);
            if (downLeft != "CantMove")
                downLeft = ConvertCheckerboardPos(x - i, y - i);
            //右上
            if (CheckerboardList.ContainsKey(upRight) && CheckerboardList[upRight] == "Empty")
                emptyPlaceList.Add(upRight);
            else
                upRight = "CantMove";
            //左上
            if (CheckerboardList.ContainsKey(upLeft) && CheckerboardList[upLeft] == "Empty")
                emptyPlaceList.Add(upLeft);
            else
                upLeft = "CantMove";
            //右下
            if (CheckerboardList.ContainsKey(downRight) && CheckerboardList[downRight] == "Empty")
                emptyPlaceList.Add(downRight);
            else
                downRight = "CantMove";
            //左下
            if (CheckerboardList.ContainsKey(downLeft) && CheckerboardList[downLeft] == "Empty")
                emptyPlaceList.Add(downLeft);
            else
                downLeft = "CantMove";
        }
        return emptyPlaceList;
    }
    public bool CheckTerrainObstacles(string location, int alertDistance, string target)
    {
        return !GetEmptyPlace(location, alertDistance).Contains(target);
    }
    public float GetDistance(string location)
    {
        int[] playerNormalPos = ConvertNormalPos(CurrentLocationID);
        int[] enemyNormalPos = ConvertNormalPos(location);
        float distance = Mathf.Sqrt(Mathf.Pow(playerNormalPos[0] - enemyNormalPos[0], 2)
         + Mathf.Pow(playerNormalPos[1] - enemyNormalPos[1], 2));
        return distance;
    }
    public void RefreshEnemyAlert()
    {
        RefreshCheckerboardList();
        for (int i = 0; i < CurrentEnemyList.Count; i++)
        {
            string location = CurrentEnemyList.ElementAt(i).Key;
            bool checkTerrainObstacles = CheckTerrainObstacles(location, CurrentEnemyList[location].AlertDistance, CurrentLocationID);
            CurrentEnemyList.ElementAt(i).Value.EnemyTrans.GetComponent<Enemy>().EnemyAlert.enabled =
            GetDistance(location) <= CurrentEnemyList[location].AlertDistance && !checkTerrainObstacles;
        }
    }
    public void RefreshCheckerboardList()
    {
        CheckerboardList.Clear();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                string location = ConvertCheckerboardPos(i, j);
                /*if (CurrentLocationID == location)
                {
                    CheckerboardList.Add(location, "Player");
                    //Debug.Log("玩家：" + loaction);
                }
                else*/
                if (CurrentEnemyList.ContainsKey(location))
                {
                    CheckerboardList.Add(location, "Enemy");
                    // Debug.Log("敵人：" + loaction);
                }
                else if (CurrentTerrainList.ContainsKey(location))
                {
                    CheckerboardList.Add(location, "Terrain");
                }
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
                break;
            case BattleType.Explore:
                Explore();
                break;
            case BattleType.BattleInitial:
                BattleInitial();
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
        CurrentLocationID = DataManager.Instance.LevelList[DataManager.Instance.LevelID].PlayerStartPos;
        int playerID = DataManager.Instance.PlayerID;
        int levelID = DataManager.Instance.LevelID;
        DataManager.Instance.PlayerList[playerID].CurrentActionPoint =
            DataManager.Instance.PlayerList[playerID].MaxActionPoint;
        DataManager.Instance.PlayerList[playerID].Mana = 5;
        PlayerTrans.anchoredPosition = CheckerboardTrans
        .GetChild(GetCheckerboardPoint(CurrentLocationID)).localPosition;
        for (int i = 0; i < DataManager.Instance.LevelList[levelID].EnemyIDList.Count; i++)
        {
            int enemyID = DataManager.Instance.LevelList[levelID].EnemyIDList.ElementAt(i).Value;
            string loactionID = DataManager.Instance.LevelList[levelID].EnemyIDList.ElementAt(i).Key;
            CurrentEnemyList.Add(loactionID, (EnemyData)DataManager.Instance.EnemyList[enemyID].Clone());
        }
        for (int i = 0; i < DataManager.Instance.LevelList[levelID].TerrainIDList.Count; i++)
        {
            int terrainID = DataManager.Instance.LevelList[levelID].TerrainIDList.ElementAt(i).Value;
            string loactionID = DataManager.Instance.LevelList[levelID].TerrainIDList.ElementAt(i).Key;
            CurrentTerrainList.Add(loactionID, DataManager.Instance.TerrainList[terrainID].Clone());
        }
        EventManager.Instance.DispatchEvent(EventDefinition.eventBattleInitial);
    }

    private void Explore()
    {
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
        EventManager.Instance.DispatchEvent(
            EventDefinition.eventExplore,
            DataManager.Instance.LevelList[DataManager.Instance.LevelID].LevelType
        );
    }

    private void Dialog()
    {
        EventManager.Instance.DispatchEvent(EventDefinition.eventDialog);
    }

    private void PlayerTurn()
    {
        int playerID = DataManager.Instance.PlayerID;
        DataManager.Instance.PlayerList[playerID].CurrentActionPoint =
        DataManager.Instance.PlayerList[playerID].MaxActionPoint;
        DataManager.Instance.PlayerList[playerID].Mana++;
        DataManager.Instance.PlayerList[playerID].CurrentShield = 0;
        CurrentNegativeState.Clear();
        for (int i = 0; i < CurrentAbilityList.Count; i++)
        {
            int id = CurrentAbilityList.ElementAt(i).Key;
            string target = CurrentAbilityList.ElementAt(i).Value;
            for (int j = 0; j < DataManager.Instance.CardList[id].CardEffectList.Count; j++)
            {
                string effectID;
                int effectCount;
                effectID = DataManager.Instance.CardList[id].CardEffectList[j].Item1;
                effectCount = DataManager.Instance.CardList[id].CardEffectList[j].Item2;
                EffectFactory.Instance.CreateEffect(effectID).ApplyEffect(effectCount, target);
            }
        }
        EventManager.Instance.DispatchEvent(EventDefinition.eventPlayerTurn);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    private void EnemyTurn()
    {
        EventManager.Instance.DispatchEvent(EventDefinition.eventEnemyTurn);
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
}
