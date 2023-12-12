using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Unity.VisualScripting;
public class UIIllustratedBook : UIBase
{
    [SerializeField]
    private Button illustratedBookButton;
    [SerializeField]
    private Transform illustratedBookTrans;
    [SerializeField]
    private CardItem cardPrefab;
    [SerializeField]
    private Button closeButton;
    private void Awake()
    {
        illustratedBookButton.onClick.AddListener(() => RefreshIllustratedBook(illustratedBookTrans, cardPrefab));
        closeButton.onClick.AddListener(Hide);
    }
    private void RefreshIllustratedBook(Transform contentTrans, CardItem cardPrefab)
    {
        Show();
        Dictionary<int, CardData> illustratedBook = DataManager.Instance.CardList;
        for (int i = 0; i < contentTrans.childCount; i++)
        {
            Destroy(contentTrans.gameObject);
        }
        for (int i = 0; i < illustratedBook.Count; i++)
        {
            CardItem cardItem = Instantiate(cardPrefab, contentTrans);
            cardItem.GetComponent<CanvasGroup>().alpha = 1;
            cardItem.CardID = illustratedBook.ElementAt(i).Key;
            cardItem.CantMove = true;
        }
    }
}
