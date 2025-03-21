using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICardMenu : UIBase
{
    [SerializeField]
    private Button cardMenuButton;
    [SerializeField]
    private Button mapCardMenuButton;

    [SerializeField]
    private Button usedCardMenuButton;
    [SerializeField]
    private Button removeCardMenuButton;
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
        cardMenuButton.onClick.AddListener(() => UIManager.Instance.RefreshCardBag(DataManager.Instance.CardBag));
        mapCardMenuButton.onClick.AddListener(() => UIManager.Instance.RefreshCardBag(DataManager.Instance.CardBag));
        usedCardMenuButton.onClick.AddListener(() => UIManager.Instance.RefreshCardBag(DataManager.Instance.UsedCardBag));
        removeCardMenuButton.onClick.AddListener(() => UIManager.Instance.RefreshCardBag(DataManager.Instance.RemoveCardBag));
        hideButton.onClick.AddListener(Hide);
        BattleManager.Instance.CardMenuTrans = contentTrans;
        BattleManager.Instance.CardBagApplyButton = applyButton;
        BattleManager.Instance.CardPrefab = cardPrefab;
    }
    public override void Hide()
    {
        base.Hide();
        Transform cardMenuTrans = BattleManager.Instance.CardMenuTrans;
        for (int i = 0; i < cardMenuTrans.childCount; i++)
        {
            Destroy(cardMenuTrans.GetChild(i).gameObject);
        }
    }
}
