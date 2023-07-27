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
        ExploreInitial,
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
    public List<EnemyData> CurrentEnemyList { get; private set; }
    public Dictionary<int, int> CurrentAbilityList { get; set; }
    public bool IsDrag { get; set; }
    public int CurrentLocationID { get; set; }

    protected override void Awake()
    {
        base.Awake();
        IsDrag = false;
        CardItemList = new List<CardItem>();
        CardPositionList = new List<Vector2>();
        CardAngleList = new List<float>();
        CurrentEnemyList = new List<EnemyData>();
        CurrentAbilityList = new Dictionary<int, int>();
    }

    private void Update()
    {
        if (MyBattleType == BattleType.None)
            ChangeTurn(BattleType.ExploreInitial);
    }

    public void TakeDamage(CharacterData defender, int damage)
    {
        int currentDamage = damage - defender.CurrentShield;
        if (currentDamage < 0)
            currentDamage = 0;
        defender.CurrentShield -= damage;
        defender.CurrentHealth -= currentDamage;
        EventManager.Instance.DispatchEvent(
            EventDefinition.eventTakeDamage,
            defender.CharacterPos,
            damage
        );
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    public void GetShield(CharacterData defender, int point)
    {
        defender.CurrentShield += point;
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    public void ConsumeActionPoint(int point)
    {
        if (
            DataManager.Instance.PlayerList[DataManager.Instance.PlayerID].CurrentActionPoint
            >= point
        )
            DataManager.Instance.PlayerList[DataManager.Instance.PlayerID].CurrentActionPoint -=
                point;
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    public void ChangeTurn(BattleType type)
    {
        MyBattleType = type;
        switch (MyBattleType)
        {
            case BattleType.None:
                break;
            case BattleType.ExploreInitial:
                ExploreInitial();
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
        DataManager.Instance.PlayerList[DataManager.Instance.PlayerID].CurrentActionPoint =
            DataManager.Instance.PlayerList[DataManager.Instance.PlayerID].MaxActionPoint;
        int id = DataManager.Instance.LevelID;
        string[] enemyStr = DataManager.Instance.LevelList[id].EnemyIDList[CurrentLocationID].Split(
            '、'
        );
        for (int i = 0; i < enemyStr.Length; i++)
        {
            CurrentEnemyList.Add(
                (EnemyData)DataManager.Instance.EnemyList[int.Parse(enemyStr[i])].Clone()
            );
        }
        EventManager.Instance.DispatchEvent(EventDefinition.eventBattleInitial);
    }

    private void ExploreInitial()
    {
        int levelID = DataManager.Instance.LevelID;
        for (int i = 0; i < DataManager.Instance.LevelList[levelID].LocationList.Count; i++)
        {
            if (
                DataManager.Instance.LevelList[levelID].LocationList.ElementAt(i).Value
                == "ENTRANCE"
            )
                CurrentLocationID = DataManager.Instance.LevelList[levelID].LocationList
                    .ElementAt(i)
                    .Key;
        }
        ChangeTurn(BattleType.Explore);
    }

    private void Explore()
    {
        EventManager.Instance.DispatchEvent(
            EventDefinition.eventExplore,
            DataManager.Instance.LevelList[DataManager.Instance.LevelID].LocationList[
                CurrentLocationID
            ]
        );
        Debug.Log("現在位置：" + CurrentLocationID);
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
            int target = CurrentAbilityList.ElementAt(i).Value;
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
            int randomIndex = UnityEngine.Random.Range(0, DataManager.Instance.CardBag.Count);
            CardData temp = DataManager.Instance.CardBag[randomIndex];
            DataManager.Instance.CardBag[randomIndex] = DataManager.Instance.CardBag[i];
            DataManager.Instance.CardBag[i] = temp;
        }
    }
}
