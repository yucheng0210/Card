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
        Initial,
        Player,
        Enemy,
        Win,
        Loss
    }

    public BattleType MyBattleType { get; set; }
    public List<Vector2> CardPositionList { get; set; }
    public List<float> CardAngleList { get; set; }
    public bool IsDrag { get; set; }
    public IEffectFactory CardEffectFactory { get; set; }

    protected override void Awake()
    {
        base.Awake();
        IsDrag = false;
        CardPositionList = new List<Vector2>();
        CardAngleList = new List<float>();
        CardEffectFactory = new EffectFactory();
    }

    private void Update()
    {
        if (MyBattleType == BattleType.None)
            ChangeTurn(BattleType.Initial);
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
            case BattleType.Initial:
                Initial();
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

    private void Initial()
    {
        DataManager.Instance.PlayerList[DataManager.Instance.PlayerID].CurrentActionPoint =
            DataManager.Instance.PlayerList[DataManager.Instance.PlayerID].MaxActionPoint;
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
        ChangeTurn(BattleType.Player);
    }

    private void PlayerTurn()
    {
        DataManager.Instance.PlayerList[DataManager.Instance.PlayerID].CurrentActionPoint =
            DataManager.Instance.PlayerList[DataManager.Instance.PlayerID].MaxActionPoint;
        EventManager.Instance.DispatchEvent(EventDefinition.eventPlayerTurn);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    private void EnemyTurn()
    {
        EventManager.Instance.DispatchEvent(EventDefinition.eventEnemyTurn);
    }

    private void Win() { }

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

    public void AddHandCard(int drawCardCount, List<CardItem> cardItems)
    {
        for (int i = 0; i < drawCardCount; i++)
        {
            DataManager.Instance.HandCard.Add(cardItems[i]);
        }
        DataManager.Instance.CardBag.RemoveRange(0, drawCardCount);
    }
}
