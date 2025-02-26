using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaknessCurseEffect : IEffect
{
    private CardItem cardItem;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        cardItem = BattleManager.Instance.AddCard(5003);
        EventManager.Instance.AddEventRegister(EventDefinition.eventAttack, EventAttack);
    }
    private void EventAttack(params object[] args)
    {
        List<CardData> handCard = DataManager.Instance.HandCard;
        if (handCard.Contains(cardItem.MyCardData))
        {
            BattleManager.Instance.PlayerMoveCount = 0;
            EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventAttack, EventAttack);
        }
    }
    public string SetTitleText()
    {
        return "虛弱";
    }
    public string SetDescriptionText()
    {
        return "抽到此卡回合移動點數歸零。";
    }

}
