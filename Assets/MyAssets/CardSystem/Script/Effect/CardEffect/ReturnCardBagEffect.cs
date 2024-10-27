using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReturnCardBag : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        UnityEngine.Events.UnityAction unityAction = Return;
        UIManager.Instance.SelectCard(unityAction, true);
        // BattleManager.Instance.CardBagApplyButton.onClick.AddListener(() => UIManager.Instance.SelectCard(unityAction, true));
    }
    public string SetTitleText()
    {
        throw new System.NotImplementedException();
    }

    public string SetDescriptionText()
    {
        throw new System.NotImplementedException();
    }

    private void Return()
    {
        BattleManager.Instance.CardBagApplyButton.gameObject.SetActive(false);
        CardItem cardItem = DataManager.Instance.UsedCardBag[UIManager.Instance.CurrentRemoveID].MyCardItem;
        cardItem.transform.SetParent(BattleManager.Instance.CardBagTrans);
        cardItem.GetComponent<RectTransform>().anchoredPosition = BattleManager.Instance.CardBagTrans.position;
        DataManager.Instance.CardBag.Insert(0, cardItem.MyCardData);
        DataManager.Instance.UsedCardBag.RemoveAt(UIManager.Instance.CurrentRemoveID);
        UIManager.Instance.HideUI("UICardMenu");
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

}
