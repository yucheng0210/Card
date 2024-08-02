using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICardMenu : UIBase
{
    [SerializeField]
    private Button cardMenuButton;
    [SerializeField]
    private Button battleCardMenuButton;

    [SerializeField]
    private Button usedCardMenuButton;
    [SerializeField]
    private Button hideButton;

    [SerializeField]
    private CardItem cardPrefab;

    [SerializeField]
    private Transform contentTrans;
    [SerializeField]
    private Button applyButton;

    protected override void Start()
    {
        base.Start();
        cardMenuButton.onClick.AddListener(() => UIManager.Instance.RefreshCardBag());
        battleCardMenuButton.onClick.AddListener(() => UIManager.Instance.RefreshCardBag());
        usedCardMenuButton.onClick.AddListener(RefreshUsedCardBag);
        hideButton.onClick.AddListener(Hide);
        BattleManager.Instance.CardBagTrans = contentTrans;
        BattleManager.Instance.CardBagApplyButton = applyButton;
        BattleManager.Instance.CardPrefab = cardPrefab;
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
