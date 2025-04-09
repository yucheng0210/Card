using System.ComponentModel;
using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
public class UIManager : Singleton<UIManager>
{
    public Dictionary<string, UIBase> UIDict { get; set; }
    public int CurrentRemoveID { get; set; }
    protected override void Awake()
    {
        base.Awake();
        UIDict = new Dictionary<string, UIBase>();
    }

    public void ShowUI(string uiName)
    {
        UIDict[uiName].Show();
    }

    public void HideUI(string uiName)
    {
        UIDict[uiName].Hide();
    }

    public void HideAllUI()
    {
        foreach (var i in UIDict)
        {
            HideUI(i.Key);
        }
    }

    public IEnumerator FadeOutIn(CanvasGroup canvasGroup, float fadeTime, float waitTime, bool canDestroy)
    {
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.unscaledDeltaTime / fadeTime;
            yield return null;
        }
        yield return new WaitForSecondsRealtime(waitTime);
        while (canvasGroup.alpha != 0)
        {
            canvasGroup.alpha -= Time.unscaledDeltaTime / fadeTime;
            yield return null;
        }
        if (canDestroy)
        {
            Destroy(canvasGroup.gameObject);
        }
    }

    public IEnumerator FadeOut(CanvasGroup canvasGroup, float fadeTime)
    {
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.unscaledDeltaTime / fadeTime;
            yield return null;
        }
    }

    public IEnumerator FadeIn(CanvasGroup canvasGroup, float fadeTime, bool canDestroy)
    {
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.unscaledDeltaTime / fadeTime;
            yield return null;
        }
        if (canDestroy)
        {
            Destroy(canvasGroup.gameObject);
        }
    }

    public Vector2 GetBezierCurve(Vector2 start, Vector2 mid, Vector2 end, float t)
    {
        return Mathf.Pow(1.0f - t, 2) * start + 2.0f * t * (1.0f - t) * mid + Mathf.Pow(t, 2) * end;
    }
    private void CreateNewItem(Item item, BackpackSlot slotPrefab, Transform slotGroupTrans)
    {
        BackpackSlot newItem = Instantiate(slotPrefab, slotGroupTrans.position, Quaternion.identity);
        newItem.gameObject.transform.SetParent(slotGroupTrans, false);
        newItem.SlotItem = item;
        newItem.SlotImage.sprite = Resources.Load<Sprite>(item.ItemImagePath);
        newItem.SlotCount.text = item.ItemHeld.ToString();
    }

    public void RefreshItem(BackpackSlot slotPrefab, Transform slotGroupTrans, Dictionary<int, Item> inventory)
    {
        // 清空現有的子物件
        for (int i = 0; i < slotGroupTrans.childCount; i++)
        {
            Destroy(slotGroupTrans.GetChild(i).gameObject);
        }

        // 使用鍵值對數組進行循環
        var keys = new List<int>(inventory.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            int key = keys[i];
            Item item = inventory[key];

            if (item.ItemHeld == 0)
            {
                // 移除物品並遞歸刷新
                inventory.Remove(key);
                RefreshItem(slotPrefab, slotGroupTrans, inventory);
                break;
            }
            else
            {
                // 創建新的物品槽
                CreateNewItem(item, slotPrefab, slotGroupTrans);
            }
        }
    }
    public void ChangeOutline(bool isEnable)
    {
        Transform cardMenuTrans = BattleManager.Instance.CardMenuTrans;
        CardItem cardItem = cardMenuTrans.GetChild(CurrentRemoveID).GetComponent<CardItem>();
        cardItem.CardOutline.SetActive(isEnable);
    }
    public void ChangeOutline(CardItem cardItem, bool isEnable)
    {
        cardItem.CardOutline.SetActive(isEnable);
    }
    public void RefreshCardBag(List<CardData> cardBag)
    {
        BattleManager.Instance.CardBagApplyButton.gameObject.SetActive(false);
        ShowUI("UICardMenu");
        Transform cardMenuTrans = BattleManager.Instance.CardMenuTrans;
        ScrollRect scrollbar = UIDict[nameof(UICardMenu)].UI.GetComponentInChildren<ScrollRect>();
        scrollbar.verticalNormalizedPosition = 1;
        for (int i = 0; i < cardMenuTrans.childCount; i++)
        {
            Destroy(cardMenuTrans.GetChild(i).gameObject);
        }
        for (int i = 0; i < cardBag.Count; i++)
        {
            CardItem cardItem = Instantiate(BattleManager.Instance.CardPrefab, cardMenuTrans);
            ChangeOutline(cardItem, false);
            cardItem.CardImage.raycastTarget = true;
            cardItem.GetComponent<CanvasGroup>().alpha = 1;
            cardItem.MyCardData = cardBag[i];
            cardItem.CantMove = true;
        }
    }
    public void ClearMoveClue(bool canRemove)
    {
        RectTransform checkerboardTrans = BattleManager.Instance.CheckerboardTrans;
        Color color = new(1, 1, 1, 0);
        for (int i = 0; i < checkerboardTrans.childCount; i++)
        {
            Image checkerboardImage = checkerboardTrans.GetChild(i).GetComponent<Image>();
            checkerboardImage.color = color;
            if (canRemove)
            {
                Button checkerboardButton = checkerboardTrans.GetChild(i).GetComponent<Button>();
                checkerboardButton.onClick.RemoveAllListeners();
            }
        }
    }
    public void ChangeCheckerboardColor(List<string> emptyPlaceList, bool isMove)
    {
        Color color = new(1, 1, 1, 1);
        for (int i = 0; i < emptyPlaceList.Count; i++)
        {
            Transform checkerboardTrans = BattleManager.Instance.GetCheckerboardTrans(emptyPlaceList[i]);
            CheckerboardSlot emptySlot = checkerboardTrans.GetComponent<CheckerboardSlot>();
            Image emptyImage = checkerboardTrans.GetComponent<Image>();
            emptyImage.sprite = isMove ? emptySlot.BlueClueImage : emptySlot.RedClueImage;
            emptyImage.color = color;
        }
    }
    public void SelectCard(UnityEngine.Events.UnityAction unityAction, bool isRefreshUseBag)
    {
        if (isRefreshUseBag)
        {
            RefreshCardBag(DataManager.Instance.UsedCardBag);
        }
        else
        {
            RefreshCardBag(DataManager.Instance.CardBag);
        }
        for (int i = 0; i < BattleManager.Instance.CardMenuTrans.childCount; i++)
        {
            int avoidClosure = i;
            Transform child = BattleManager.Instance.CardMenuTrans.GetChild(i);
            // 檢查是否已有 Button，沒有才新增
            Button cardButton = child.GetComponent<Button>() ?? child.gameObject.AddComponent<Button>();
            cardButton.onClick.RemoveAllListeners();
            cardButton.onClick.AddListener(() => RefreshCardSelect(avoidClosure, unityAction));
        }
    }
    private void RefreshCardSelect(int removeID, UnityEngine.Events.UnityAction unityAction)
    {
        Button cardBagApplyButton = BattleManager.Instance.CardBagApplyButton;
        Transform cardMenuTrans = BattleManager.Instance.CardMenuTrans;
        cardBagApplyButton.gameObject.SetActive(true);
        cardBagApplyButton.onClick.RemoveAllListeners();
        CurrentRemoveID = removeID;
        for (int i = 0; i < cardMenuTrans.childCount; i++)
        {
            CardItem cardItem = cardMenuTrans.GetChild(i).GetComponent<CardItem>();
            ChangeOutline(cardItem, false);
        }
        if (CurrentRemoveID < cardMenuTrans.childCount && CurrentRemoveID != -1)
        {
            ChangeOutline(true);
            cardBagApplyButton.onClick.AddListener(unityAction);
        }
    }

}
