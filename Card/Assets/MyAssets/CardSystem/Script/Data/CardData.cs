using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData
{
    private int cardCost;
    public int CardID { get; set; }
    public string CardName { get; set; }
    public string CardType { get; set; }
    public string CardImagePath { get; set; }
    public int CardCost
    {
        get { return cardCost; }
        set
        {
            cardCost = value;
            if (cardCost < 0)
                cardCost = 0;
            if (CardCost > 9)
                cardCost = 9;
        }
    }
    public string CardAttribute { get; set; }
    public string CardEffect { get; set; }
    public string CardDescription { get; set; }
    public int cardHeld { get; set; }
}
