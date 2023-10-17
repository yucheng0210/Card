using System.ComponentModel;
using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class UIManager : Singleton<UIManager>
{
    public Dictionary<string, UIBase> UIDict { get; set; }

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

    public IEnumerator FadeOutIn(
        CanvasGroup canvasGroup,
        float fadeTime,
        float waitTime,
        bool canDestroy
    )
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
        return Mathf.Pow(1.0f - t, 2) * start
            + 2.0f * t * (1.0f - t) * mid
            + Mathf.Pow(t, 2) * end;
    }
    private void CreateNewItem(Item item, BackpackSlot slotPrefab, Transform slotGroupTrans)
    {
        BackpackSlot newItem = Instantiate(
            slotPrefab,
            slotGroupTrans.position,
            Quaternion.identity
        );
        newItem.gameObject.transform.SetParent(slotGroupTrans, false);
        newItem.SlotItem = item;
        newItem.SlotImage.sprite = Resources.Load<Sprite>(item.ItemImagePath);
        newItem.SlotCount.text = item.ItemHeld.ToString();
    }

    /*   private void CreateNewItem(Quest quest, QuestSlot slotPrefab, Transform slotGroupTrans)
       {
           QuestSlot newItem = Instantiate(slotPrefab, slotGroupTrans.position, Quaternion.identity);
           newItem.gameObject.transform.SetParent(slotGroupTrans, false);
           newItem.MyQuest = quest;
           newItem.SlotName.text = quest.TheName;
           newItem.NPCName.text = quest.NPC;
       }*/

    public void RefreshItem(
        BackpackSlot slotPrefab,
        Transform slotGroupTrans,
        Dictionary<int, Item> inventory
    )
    {
        for (int i = 0; i < slotGroupTrans.childCount; i++)
            Destroy(slotGroupTrans.GetChild(i).gameObject);
        foreach (KeyValuePair<int, Item> i in inventory)
        {
            if (i.Value.ItemHeld == 0)
            {
                inventory.Remove(i.Key);
                RefreshItem(slotPrefab, slotGroupTrans, inventory);
                break;
            }
            else
                CreateNewItem(i.Value, slotPrefab, slotGroupTrans);
        }
    }
    public void ChangeOutline(Outline outline, float length)
    {
        outline.effectDistance = new Vector2(length, length);
    }
    public void RefreshCardBag(Transform contentTrans,CardItem cardPrefab)
    {
      ShowUI("UICardMenu");
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
    public IEnumerator RefreshEnemyAlert()
    {
        BattleManager.Instance.RefreshCheckerboardList();
        yield return null;
        for (int i = 0; i < BattleManager.Instance.CurrentEnemyList.Count; i++)
        {
            string location = BattleManager.Instance.CurrentEnemyList.ElementAt(i).Key;
            bool checkTerrainObstacles = BattleManager.Instance.CheckTerrainObstacles(location, BattleManager.Instance.CurrentEnemyList[location].AlertDistance,  BattleManager.Instance.CurrentLocationID);
            BattleManager.Instance.CurrentEnemyList.ElementAt(i).Value.EnemyTrans.GetComponent<Enemy>().EnemyAlert.enabled =
            BattleManager.Instance.GetDistance(location) <= BattleManager.Instance.CurrentEnemyList[location].AlertDistance && !checkTerrainObstacles;
        }
    }
}
