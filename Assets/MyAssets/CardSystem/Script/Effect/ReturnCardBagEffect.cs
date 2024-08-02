using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnCardBag : IEffect
{
    public void ApplyEffect(int value, string target)
    {
        UnityEngine.Events.UnityAction unityAction = Return;
        BattleManager.Instance.CardBagApplyButton.onClick.AddListener(() => UIManager.Instance.SelectCard(unityAction));
    }
    public Sprite SetIcon()
    {
        throw new System.NotImplementedException();
    }
    private void Return()
    {
      /*  BattleManager.Instance.CardBagApplyButton.onClick.RemoveAllListeners();
        BattleManager.Instance.CardBagApplyButton.gameObject.SetActive(false);
        DataManager.Instance.UsedCardBag.RemoveAt(UIManager.Instance.CurrentRemoveID);
        DataManager.Instance.CardBag.Insert(0, DataManager.Instance.CardBag[UIManager.Instance.CurrentRemoveID]);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);*/
    }

}
