using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICardMenu : UIBase
{
    [SerializeField]
    private Button cardMenuButton;

    [SerializeField]
    private Button usedCardMenuButton;

    [SerializeField]
    private CardItem cardPrefab;

    [SerializeField]
    private Transform contentTrans;

    private void Awake()
    {
        cardMenuButton.onClick.AddListener(RefreshCardBag);
        usedCardMenuButton.onClick.AddListener(RefreshUsedCardBag);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && UI.activeSelf)
            UI.SetActive(false);
    }

    private void RefreshCardBag()
    {
        UI.SetActive(true);
        List<CardData> cardBag = DataManager.Instance.CardBag;
        for (int i = 0; i < contentTrans.childCount; i++)
        {
            Destroy(contentTrans.GetChild(i).gameObject);
        }
        for (int i = 0; i < cardBag.Count; i++)
        {
            CardItem cardItem = Instantiate(cardPrefab, contentTrans);
            cardItem.GetComponent<CanvasGroup>().alpha = 1;
            cardItem.CardID = cardBag[i].CardID;
            cardItem.CantMove = true;
        }
    }

    private void RefreshUsedCardBag()
    {
        UI.SetActive(true);
        for (int i = 0; i < contentTrans.childCount; i++)
        {
            Destroy(contentTrans.GetChild(i).gameObject);
        }
        List<CardItem> usedCardBag = DataManager.Instance.UsedCardBag;
        for (int i = 0; i < usedCardBag.Count; i++)
        {
            CardItem cardItem = Instantiate(cardPrefab, contentTrans);
            cardItem.GetComponent<CanvasGroup>().alpha = 1;
            cardItem.CardID = usedCardBag[i].CardID;
            cardItem.CantMove = true;
        }
    }
}
