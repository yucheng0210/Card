using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInventory", menuName = "Card/NewInventory")]
public class Inventory_SO : ScriptableObject
{
    [SerializeField]
    private List<Card_SO> inventory = new List<Card_SO>();
    #region  "Read for Inventory_SO"
    public List<Card_SO> Inventory
    {
        get { return inventory; }
    }
    #endregion
}
