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
            Destroy(canvasGroup.gameObject);
    }

    public IEnumerator FadeOut(CanvasGroup canvasGroup, float fadeTime)
    {
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.unscaledDeltaTime / fadeTime;
            yield return null;
        }
    }

    public IEnumerator FadeIn(CanvasGroup canvasGroup, float fadeTime)
    {
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.unscaledDeltaTime / fadeTime;
            yield return null;
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
    public void ChangeOutline(Outline outline, float length)
    {
        outline.effectDistance = new Vector2(length, length);
    }
    public void RefreshCardBag()
    {
        ShowUI("UICardMenu");
        List<CardData> cardBag = DataManager.Instance.CardBag;
        for (int i = 0; i < BattleManager.Instance.CardBagTrans.childCount; i++)
        {
            Destroy(BattleManager.Instance.CardBagTrans.GetChild(i).gameObject);
        }
        for (int i = 0; i < cardBag.Count; i++)
        {
            CardItem cardItem = Instantiate(BattleManager.Instance.CardPrefab, BattleManager.Instance.CardBagTrans);
            cardItem.GetComponent<CanvasGroup>().alpha = 1;
            cardItem.CardID = cardBag[i].CardID;
            cardItem.CantMove = true;
        }
    }

    public void ClearMoveClue(bool canRemove)
    {
        RectTransform checkerboardTrans = BattleManager.Instance.CheckerboardTrans;
        for (int i = 0; i < checkerboardTrans.childCount; i++)
        {
            checkerboardTrans.GetChild(i).GetComponent<Image>().color = new Color(1, 1, 1, 0);
            if (canRemove)
                checkerboardTrans.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }
    public void ChangeCheckerboardColor(Color color, string location, int stepCount, BattleManager.CheckEmptyType checkEmptyType)
    {
        //ClearMoveClue(false);
        List<string> emptyPlaceList = BattleManager.Instance.GetEmptyPlace(location, stepCount, checkEmptyType);
        for (int i = 0; i < emptyPlaceList.Count; i++)
        {
            int checkerboardPoint = BattleManager.Instance.GetCheckerboardPoint(emptyPlaceList[i]);
            RectTransform emptyPlace = BattleManager.Instance.CheckerboardTrans.GetChild(checkerboardPoint).GetComponent<RectTransform>();
            emptyPlace.GetComponent<Image>().color = color;
        }
    }
    public void ClearCheckerboardColor(string location, int stepCount, BattleManager.CheckEmptyType checkEmptyType)
    {
        //ClearMoveClue(false);
        List<string> emptyPlaceList = BattleManager.Instance.GetEmptyPlace(location, stepCount, checkEmptyType);
        Color color = new Color(1, 1, 1, 0);
        for (int i = 0; i < emptyPlaceList.Count; i++)
        {
            int checkerboardPoint = BattleManager.Instance.GetCheckerboardPoint(emptyPlaceList[i]);
            RectTransform emptyPlace = BattleManager.Instance.CheckerboardTrans.GetChild(checkerboardPoint).GetComponent<RectTransform>();
            emptyPlace.GetComponent<Image>().color = color;
        }
    }
    public void SelectCard(UnityEngine.Events.UnityAction unityAction)
    {
        RefreshCardBag();
        for (int i = 0; i < BattleManager.Instance.CardBagTrans.childCount; i++)
        {
            int avoidClosure = i;
            Button cardButton = BattleManager.Instance.CardBagTrans.GetChild(i).gameObject.AddComponent<Button>();
            cardButton.onClick.AddListener(() => RefreshCardSelect(avoidClosure, unityAction));
        }
    }
    private void RefreshCardSelect(int removeID, UnityEngine.Events.UnityAction unityAction)
    {
        BattleManager.Instance.CardBagApplyButton.gameObject.SetActive(true);
        // exitButton.gameObject.SetActive(true);
        CurrentRemoveID = removeID;
        BattleManager.Instance.CardBagApplyButton.onClick.RemoveAllListeners();
        for (int i = 0; i < BattleManager.Instance.CardBagTrans.childCount; i++)
        {
            ChangeOutline(BattleManager.Instance.CardBagTrans.GetChild(i).GetComponentInChildren<Outline>(), 0);
        }
        if (CurrentRemoveID < BattleManager.Instance.CardBagTrans.childCount && CurrentRemoveID != -1)
        {
            ChangeOutline(BattleManager.Instance.CardBagTrans.GetChild(CurrentRemoveID).GetComponentInChildren<Outline>(), 6);
            BattleManager.Instance.CardBagApplyButton.onClick.AddListener(unityAction);
        }
    }

}
