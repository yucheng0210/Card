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
        Enemy,
        Win,
        Victory,
        Loss
    }

    public BattleType MyBattleType { get; set; }
    public List<CardItem> CardItemList { get; set; }
    public List<Vector2> CardPositionList { get; set; }
    public List<float> CardAngleList { get; set; }
    public Dictionary<int, string> CurrentAbilityList { get; set; }
    //敵人
    public Dictionary<string, EnemyData> CurrentEnemyList { get; set; }
    public bool IsDrag { get; set; }
    //棋盤
    public string CurrentLocationID { get; set; }
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
    public int[] ConvertNormalPos(string loaction)
    {
        string[] myLocation = loaction.Split(' ');
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
    public List<string> GetEmptyPlace(string loaction, int stepCount)
    {
        List<string> emptyPlaceList = new();
        int[] pos = ConvertNormalPos(loaction);
        int x = pos[0];
        int y = pos[1];
        for (int i = 1; i <= stepCount; i++)
        {
            emptyPlaceList.Add(ConvertCheckerboardPos(x, y + i));
            emptyPlaceList.Add(ConvertCheckerboardPos(x, y - i));
            emptyPlaceList.Add(ConvertCheckerboardPos(x + i, y));
            emptyPlaceList.Add(ConvertCheckerboardPos(x - i, y));
            if (i == stepCount)
                break;
            emptyPlaceList.Add(ConvertCheckerboardPos(x + i, y + i));
            emptyPlaceList.Add(ConvertCheckerboardPos(x + i, y - i));
            emptyPlaceList.Add(ConvertCheckerboardPos(x - i, y + i));
            emptyPlaceList.Add(ConvertCheckerboardPos(x - i, y - i));
        }
        List<string> newEmptyPlaceList = new();
        for (int i = 0; i < emptyPlaceList.Count; i++)
        {
            if (CheckerboardList.ContainsKey(emptyPlaceList[i]) && CheckerboardList[emptyPlaceList[i]] == "Empty")
                newEmptyPlaceList.Add(emptyPlaceList[i]);
        }
        emptyPlaceList = newEmptyPlaceList;
        return emptyPlaceList;
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
            CurrentEnemyList.ElementAt(i).Value.EnemyTrans.GetComponent<Enemy>().EnemyAlert.enabled =
            GetDistance(location) <= CurrentEnemyList[location].AttackDistance;
        }
    }
    public void RefreshCheckerboardList()
    {
        CheckerboardList.Clear();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                string loaction = ConvertCheckerboardPos(i, j);
                if (CurrentLocationID == loaction)
                {
                    CheckerboardList.Add(loaction, "Player");
                    //Debug.Log("玩家：" + loaction);
                }
                else if (CurrentEnemyList.ContainsKey(loaction))
                {
                    CheckerboardList.Add(loaction, "Enemy");
                   // Debug.Log("敵人：" + loaction);
                }
                else
                    CheckerboardList.Add(loaction, "Empty");
            }
        }
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
        PlayerTrans.anchoredPosition = CheckerboardTrans
        .GetChild(GetCheckerboardPoint(CurrentLocationID)).localPosition;
        for (int i = 0; i < DataManager.Instance.LevelList[levelID].EnemyIDList.Count; i++)
        {
            int enemyID = DataManager.Instance.LevelList[levelID].EnemyIDList.ElementAt(i).Value;
            string loactionID = DataManager.Instance.LevelList[levelID].EnemyIDList.ElementAt(i).Key;
            CurrentEnemyList.Add(loactionID, (EnemyData)DataManager.Instance.EnemyList[enemyID].Clone());
        }
        RefreshCheckerboardList();
        EventManager.Instance.DispatchEvent(EventDefinition.eventBattleInitial);
    }

    private void Explore()
    {
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
        DataManager.Instance.PlayerList[DataManager.Instance.PlayerID].CurrentActionPoint =
            DataManager.Instance.PlayerList[DataManager.Instance.PlayerID].MaxActionPoint;
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
