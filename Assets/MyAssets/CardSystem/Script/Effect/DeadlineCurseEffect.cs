using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadlineCurseEffect : IEffect
{
    private CardItem cardItem;
    private int reciprocalCount = 3;
    public void ApplyEffect(int value, string target)
    {
        cardItem = BattleManager.Instance.AddCard(5001);
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
    }
    private void EventPlayerTurn(params object[] args)
    {
        reciprocalCount--;
        if (reciprocalCount <= 0)
        {
            if (DataManager.Instance.HandCard.Contains(cardItem))
            {
                string locationID = BattleManager.Instance.CurrentLocationID;
                PlayerData playerData = DataManager.Instance.PlayerList[DataManager.Instance.PlayerID];
                BattleManager.Instance.TakeDamage(playerData, DataManager.Instance.CardList[5001].CardAttack, locationID);
                DataManager.Instance.HandCard.Remove(cardItem);
                DataManager.Instance.RemoveCardBag.Add(cardItem);
                cardItem.gameObject.SetActive(false);
            }
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
        }
    }
    public Sprite SetIcon()
    {
        return Resources.Load<Sprite>("CardImage/FireBall");
    }
}