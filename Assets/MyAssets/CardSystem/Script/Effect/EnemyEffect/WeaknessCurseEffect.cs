using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaknessCurseEffect : IEffect
{
    private CardItem cardItem;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        cardItem = BattleManager.Instance.AddCard(5003);
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
    }
    private void EventPlayerTurn(params object[] args)
    {
        List<CardData> handCard = DataManager.Instance.HandCard;
        if (handCard.Contains(cardItem.MyCardData))
        {
            BattleManager.Instance.PlayerMoveCount = 0;
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
        }
    }
    public string SetTitleText()
    {
        throw new System.NotImplementedException();
    }
    public string SetDescriptionText()
    {
        throw new System.NotImplementedException();
    }

}
