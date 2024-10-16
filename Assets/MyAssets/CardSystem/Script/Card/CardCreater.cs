using System.Data;
using System.Net.Mime;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Reflection.Emit;

public class CardCreater : MonoBehaviour
{
    [SerializeField]
    private Vector2 startPosition = new Vector2(878, -20);

    [SerializeField]
    private Transform handCardTrans;

    [SerializeField]
    private Transform usedCardTrans;

    [SerializeField]
    private float coolDown;

    [SerializeField]
    private float cardXSpacing;

    [SerializeField]
    private float cardYSpacing;

    [SerializeField]
    private float minCardAngle;

    [SerializeField]
    private float moveTime;
    private float currentPosX;

    private void Start()
    {
        //StartCoroutine(DrawCard());
        EventManager.Instance.AddEventRegister(EventDefinition.eventUseCard, EventUseCard);
        EventManager.Instance.AddEventRegister(EventDefinition.eventEnemyTurn, EventEnemyTurn);
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
        EventManager.Instance.AddEventRegister(EventDefinition.eventBattleWin, EventBattleWin);
        EventManager.Instance.AddEventRegister(EventDefinition.eventBattleInitial, EventBattleInitial);
        EventManager.Instance.AddEventRegister(EventDefinition.eventDrawCard, EventDrawCard);
        BattleManager.Instance.CardBagTrans = transform;
    }

    private void CreateCard()
    {
        List<CardData> cardBag = DataManager.Instance.CardBag;
        BattleManager.Instance.Shuffle(); // 在這之前將卡片順序洗牌
        for (int i = 0; i < cardBag.Count; i++)
        {
            CardItem cardItem = Instantiate(BattleManager.Instance.CardPrefab, transform);
            cardItem.CardID = cardBag[i].CardID;
            cardItem.gameObject.SetActive(false);
            BattleManager.Instance.CardItemList.Add(cardItem);
        }
    }

    private void CalculatePositionAngle(int cardCount)
    {
        BattleManager.Instance.CardPositionList.Clear();
        BattleManager.Instance.CardAngleList.Clear();
        int odd = cardCount % 2 != 0 ? 0 : 1;
        float startAngle = (cardCount / 2 - odd) * minCardAngle;
        Vector2 cardPos = startPosition;
        //當偶數時startPosition會在中間兩張牌之間，需要扣除額外一半cardXSpacing；
        cardPos.x -= ((cardCount / 2 - odd) + (0.5f * odd)) * cardXSpacing;
        cardPos.y -= ((1 + (cardCount / 2 - odd)) * (cardCount / 2 - odd)) / 2 * cardYSpacing;
        for (int i = 0; i < cardCount; i++)
        {
            BattleManager.Instance.CardPositionList.Add(cardPos);
            BattleManager.Instance.CardAngleList.Add(startAngle);
            int ySpacingMultiplier = 0;
            if (odd == 0)
            {
                if (i >= cardCount / 2 - odd)
                    ySpacingMultiplier = cardCount / 2 - odd - i - 1;
                else
                    ySpacingMultiplier = cardCount / 2 - odd - i;
                startAngle -= minCardAngle; // 更新下一張卡片的旋轉角度
            }
            else
            {
                if (!(i == cardCount / 2 - odd))
                {
                    startAngle -= minCardAngle; // 更新下一張卡片的旋轉角度
                    ySpacingMultiplier = cardCount / 2 - odd - i;
                }
            }
            cardPos.x += cardXSpacing;
            cardPos.y += ySpacingMultiplier * cardYSpacing;
        }
    }

    private IEnumerator DrawCard(int addCardCount)
    {
        //yield return UIManager.Instance.FadeOutIn(roundTip.GetComponent<CanvasGroup>(), 0.5f, 1, false); // 執行 UI 淡入淡出效果
        yield return new WaitForSecondsRealtime(1);
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.DrawCard);
        List<CardItem> handCard = DataManager.Instance.HandCard;
        int maxCardCount = handCard.Count + addCardCount;
        // 檢查卡片數量是否為奇數
        CalculatePositionAngle(maxCardCount);
        // 逐個處理每張手牌卡片
        for (int i = handCard.Count; i < maxCardCount; i++)
        {
            if (DataManager.Instance.CardBag.Count == 0)
                DrawnAllCards();
            // 根據抽卡數量將卡片添加到手牌中
            handCard.Add(BattleManager.Instance.CardItemList[0]);
            yield return new WaitForSecondsRealtime(coolDown); // 等待冷卻時間
            RectTransform handCardRect = handCard[i].GetComponent<RectTransform>();
            handCard[i].transform.SetParent(handCardTrans); // 將卡片設定為手牌的子物件
            handCard[i].GetComponent<Image>().raycastTarget = false;
            handCard[i].gameObject.SetActive(true); // 啟用卡片物件
            EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
            yield return null;
            handCard[i].CantMove = false;
            StartCoroutine(UIManager.Instance.FadeOut(handCard[i].GetComponent<CanvasGroup>(), moveTime));
            handCardRect.DOAnchorPosX(currentPosX, moveTime); // 使用 DOTween 動畫設定卡片位置
            handCardRect.DOAnchorPosY(BattleManager.Instance.CardPositionList[i].y, moveTime);
            handCardRect.DORotateQuaternion(Quaternion.Euler(0, 0, BattleManager.Instance.CardAngleList[i]), moveTime);

            // 將前面的卡片向左偏移以創造重疊效果
            for (int j = i; j > 0; j--)
            {
                RectTransform lastHandCardRect = handCard[j - 1].GetComponent<RectTransform>();
                lastHandCardRect.DOAnchorPosX(currentPosX - (i - j + 1) * cardXSpacing, moveTime);
                lastHandCardRect.DOAnchorPosY(BattleManager.Instance.CardPositionList[j - 1].y, moveTime);
                lastHandCardRect.DORotateQuaternion(Quaternion.Euler(0, 0, BattleManager.Instance.CardAngleList[j - 1]), moveTime);
            }
            currentPosX += cardXSpacing / 2; // 更新下一張卡片的起始位置
            // 設定卡片旋轉角度
            DataManager.Instance.CardBag.RemoveAt(0);
            BattleManager.Instance.CardItemList.RemoveAt(0);
            EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
        }
        yield return new WaitForSecondsRealtime(moveTime);
        HandCardRaycast();
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Attack);
    }

    private void HandCardRaycast()
    {
        List<CardItem> handCard = DataManager.Instance.HandCard;
        for (int i = 0; i < handCard.Count; i++)
        {
            handCard[i].GetComponent<Image>().raycastTarget = true;
        }
    }
    private void DrawnAllCards()
    {
        for (int j = 0; j < DataManager.Instance.UsedCardBag.Count; j++)
        {
            BattleManager.Instance.CardItemList.Add(DataManager.Instance.UsedCardBag[j]);
            int index = DataManager.Instance.UsedCardBag[j].CardID;
            DataManager.Instance.CardBag.Add(DataManager.Instance.CardList[index]);
        }
        for (int j = 0; j < BattleManager.Instance.CardItemList.Count; j++)
        {
            BattleManager.Instance.CardItemList[j].transform.SetParent(transform);
            BattleManager.Instance.CardItemList[j].GetComponent<RectTransform>().anchoredPosition = transform.position;
        }
        DataManager.Instance.UsedCardBag.Clear();
        BattleManager.Instance.Shuffle();
    }

    private IEnumerator HideAllCards()
    {
        for (int i = 0; i < DataManager.Instance.UsedCardBag.Count; i++)
        {
            StartCoroutine(UIManager.Instance.FadeIn(DataManager.Instance.UsedCardBag[i].GetComponent<CanvasGroup>(), moveTime / 2));
            yield return null;
        }
        yield return new WaitForSecondsRealtime(moveTime);
        for (int i = 0; i < DataManager.Instance.UsedCardBag.Count; i++)
        {
            DataManager.Instance.UsedCardBag[i].gameObject.SetActive(false);
            yield return null;
        }
    }


    private void EventDrawCard(params object[] args)
    {
        StartCoroutine(DrawCard((int)(args[0])));
    }

    private void EventUseCard(params object[] args)
    {
        currentPosX -= cardXSpacing / 2;
        CardItem cardItem = (CardItem)args[0];
        cardItem.transform.SetParent(usedCardTrans);
        AdjustCard();
    }

    private void EventBattleInitial(params object[] args)
    {
        CreateCard();
    }

    private void EventPlayerTurn(params object[] args)
    {
        currentPosX = startPosition.x;
        StartCoroutine(DrawCard(BattleManager.Instance.CurrentDrawCardCount));
    }

    private void AdjustCard()
    {
        List<CardItem> handCard = DataManager.Instance.HandCard;
        CalculatePositionAngle(handCard.Count);
        for (int i = 0; i < handCard.Count; i++)
        {
            RectTransform handCardRect = handCard[i].GetComponent<RectTransform>();
            handCardRect.DOAnchorPos(BattleManager.Instance.CardPositionList[i], moveTime);
            handCardRect.DORotateQuaternion(Quaternion.Euler(0, 0, BattleManager.Instance.CardAngleList[i]), moveTime);
        }
    }


    private void EventEnemyTurn(params object[] args)
    {
        List<CardItem> freezeCardList = new List<CardItem>();
        List<CardItem> handCard = DataManager.Instance.HandCard;
        for (int i = 0; i < handCard.Count; i++)
        {
            if (DataManager.Instance.CardList[handCard[i].CardID].CardFreeze)
            {
                freezeCardList.Add(handCard[i]);
                continue;
            }
            handCard[i].gameObject.SetActive(true);
            handCard[i].CantMove = true;
            handCard[i].transform.SetParent(usedCardTrans);
            handCard[i].GetComponent<RectTransform>().DOAnchorPos(usedCardTrans.GetComponent<RectTransform>().anchoredPosition, moveTime);
            DataManager.Instance.UsedCardBag.Add(handCard[i]);
        }
        handCard.Clear();
        handCard = freezeCardList;
        AdjustCard();
        StartCoroutine(HideAllCards());
    }

    private void EventBattleWin(params object[] args)
    {
        List<CardItem> cardItemList = BattleManager.Instance.CardItemList;
        List<CardItem> handCard = DataManager.Instance.HandCard;
        List<CardItem> useCardBag = DataManager.Instance.UsedCardBag;
        List<CardItem> removeCardBag = DataManager.Instance.RemoveCardBag;
        Dictionary<int, CardData> cardList = DataManager.Instance.CardList;
        List<CardData> cardBag = DataManager.Instance.CardBag;
        for (int i = 0; i < handCard.Count; i++)
        {
            cardItemList.Add(handCard[i]);
            int index = handCard[i].CardID;
            if (cardList[index].CardType == "詛咒")
                continue;
            cardBag.Add(cardList[index]);
        }
        for (int j = 0; j < useCardBag.Count; j++)
        {
            cardItemList.Add(useCardBag[j]);
            int index = useCardBag[j].CardID;
            if (cardList[index].CardType == "詛咒")
                continue;
            cardBag.Add(cardList[index]);
        }
        for (int j = 0; j < removeCardBag.Count; j++)
        {
            cardItemList.Add(removeCardBag[j]);
            int index = removeCardBag[j].CardID;
            if (cardList[index].CardType == "詛咒")
                continue;
            cardBag.Add(cardList[index]);
        }
        for (int j = 0; j < cardItemList.Count; j++)
        {
            Destroy(cardItemList[j].gameObject);
        }
        BattleManager.Instance.CardAngleList.Clear();
        BattleManager.Instance.CardPositionList.Clear();
        removeCardBag.Clear();
        useCardBag.Clear();
        handCard.Clear();
        cardItemList.Clear();
    }
}
