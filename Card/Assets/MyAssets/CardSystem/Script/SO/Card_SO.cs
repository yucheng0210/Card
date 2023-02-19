using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Card/NewCard")]
public class Card_SO : ScriptableObject
{
    public enum CardType
    {
        Attack,
        Defend
    }

    public CardType cardType;

    [SerializeField]
    private string cardName;

    [SerializeField]
    private Sprite cardImage;

    [SerializeField]
    private int cardHeld;

    [SerializeField]
    [TextArea]
    private string cardDescription;
    #region "Read from Card_SO"
    public string CardName
    {
        get { return cardName; }
    }
    public Sprite CardImage
    {
        get { return cardImage; }
    }
    public int CardHeld
    {
        get { return cardHeld; }
        set
        {
            cardHeld = value;
            if (cardHeld > 99)
                cardHeld = 99;
        }
    }
    public string CardDescription
    {
        get { return cardDescription; }
    }
    #endregion
}
