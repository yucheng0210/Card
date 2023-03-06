using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData_So : ScriptableObject
{
    private int cardCost;
    private int cardAttack;
    private int cardDefend;
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
    public int CardHeld { get; set; }
    public int CardAttack
    {
        get { return cardAttack; }
        set
        {
            cardAttack = value;
            if (cardAttack < 0)
                cardAttack = 0;
        }
    }
    public int CardDefend
    {
        get { return cardDefend; }
        set
        {
            cardDefend = value;
            if (cardDefend < 0)
                cardDefend = 0;
        }
    }
    public string CardScript { get; set; }
}
