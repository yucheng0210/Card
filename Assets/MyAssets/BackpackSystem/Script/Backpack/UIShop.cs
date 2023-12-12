using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class UIShop : UIBase
{
    [SerializeField]
    private Transform cardGroupTrans;
    [SerializeField]
    private Transform potionGroupTrans;
    [SerializeField]
    private Transform itemGroupTrans;
    [SerializeField]
    private CardItem cardPrefab;

    private void RefreshMerchandise()
    {
        for (int i = 0; i < cardGroupTrans.childCount; i++)
        {
            Destroy(cardGroupTrans.gameObject);
        }
        for (int i = 0; i < 9; i++)
        {
            int randomIndex = Random.Range(0, DataManager.Instance.CardList.Count);
            CardItem cardItem = Instantiate(cardPrefab, cardGroupTrans);
            cardItem.GetComponent<CanvasGroup>().alpha = 1;
            cardItem.CardID = DataManager.Instance.CardList.ElementAt(randomIndex).Key;
            cardItem.CantMove = true;
        }
    }
}
