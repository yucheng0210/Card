using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData
{
    private int cardCost;
    private int cardAttack;
    private int cardShield;
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
    public string CardSpecialEffect { get; set; }
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
    public int CardShield
    {
        get { return cardShield; }
        set
        {
            cardShield = value;
            if (cardShield < 0)
                cardShield = 0;
        }
    }
    public List<ValueTuple<string, int>> CardEffectList { get; set; }
    public string CardRarity { get; set; }
}
