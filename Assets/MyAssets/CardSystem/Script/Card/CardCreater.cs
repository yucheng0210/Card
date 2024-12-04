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
        BattleManager.Instance.UseCardBagTrans = usedCardTrans;
    }

    private void CreateCard()
    {
        List<CardData> cardBag = DataManager.Instance.CardBag;
        BattleManager.Instance.Shuffle(); // 在這之前將卡片順序洗牌
        for (int i = 0; i < cardBag.Count; i++)
        {
            CardItem cardItem = Instantiate(BattleManager.Instance.CardPrefab, transform);
            RectTransform rectTransform = cardItem.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = transform.position;
            cardItem.MyCardData = cardBag[i];
            cardItem.gameObject.SetActive(false);
            cardBag[i].MyCardItem = cardItem;
        }
    }

    private void CalculatePositionAngle(int cardCount, List<CardData> cardBag)
    {
        int adjustCount = Mathf.Min(cardCount, cardBag.Count);
        // 計算是否為偶數，並確定開始角度
        int isOddCount = adjustCount % 2 != 0 ? 0 : 1;
        float startAngle = (adjustCount / 2 - isOddCount) * minCardAngle;
        // 計算初始位置，處理偶數牌位偏移
        Vector2 cardPos = startPosition;
        float halfCardCount = adjustCount / 2 - isOddCount;
        cardPos.x -= (halfCardCount + (float)(isOddCount / 2f)) * cardXSpacing;
        cardPos.y -= (1 + halfCardCount) * (adjustCount / 2f - isOddCount) / 2 * cardYSpacing;
        // 確定迴圈次數，避免超過卡包數量
        for (int i = 0; i < adjustCount; i++)
        {
            // 獲取當前卡片
            CardItem cardItem = cardBag[i].MyCardItem;
            // 設置當前卡片位置與角度
            cardItem.CurrentPos = cardPos;
            cardItem.CurrentAngle = startAngle;
            // 計算y軸偏移量
            int ySpacingMultiplier = adjustCount / 2 - i - isOddCount;
            if (isOddCount == 0)
            {
                if (i >= adjustCount / 2)
                {
                    ySpacingMultiplier--;
                }
            }
            else
            {
                if (i == adjustCount / 2 - isOddCount)
                {
                    startAngle += minCardAngle;
                }
            }
            // 更新下一張卡片的x和y座標，以及角度
            cardPos.x += cardXSpacing;
            cardPos.y += ySpacingMultiplier * cardYSpacing;
            startAngle -= minCardAngle;  // 更新旋轉角度
        }
    }
    private IEnumerator DrawCard(int addCardCount)
    {
        yield return new WaitForSecondsRealtime(1.5f);
        List<CardData> handCard = DataManager.Instance.HandCard;
        List<CardData> cardBag = DataManager.Instance.CardBag;
        int maxCardCount = handCard.Count + addCardCount;
        CalculatePositionAngle(maxCardCount, cardBag);
        for (int i = handCard.Count; i < maxCardCount; i++)
        {
            if (cardBag.Count == 0)
            {
                if (DataManager.Instance.UsedCardBag.Count == 0)
                {
                    break;
                }
                DrawnAllCards(maxCardCount - i);
            }
            // 根據抽卡數量將卡片添加到手牌中
            handCard.Add(cardBag[0]);
            yield return new WaitForSecondsRealtime(coolDown); // 等待冷卻時間
            CardItem cardItem = handCard[i].MyCardItem;
            RectTransform handCardRect = cardItem.GetComponent<RectTransform>();
            cardItem.transform.SetParent(handCardTrans); // 將卡片設定為手牌的子物件
            cardItem.CardCollision.raycastTarget = false;
            cardItem.gameObject.SetActive(true); // 啟用卡片物件
            EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
            yield return null;
            cardItem.CantMove = false;
            StartCoroutine(UIManager.Instance.FadeOut(cardItem.GetComponent<CanvasGroup>(), moveTime));
            handCardRect.DOAnchorPosX(currentPosX, moveTime); // 使用 DOTween 動畫設定卡片位置
            handCardRect.DOAnchorPosY(cardItem.CurrentPos.y, moveTime);
            handCardRect.DORotateQuaternion(Quaternion.Euler(0, 0, cardItem.CurrentAngle), moveTime);

            // 將前面的卡片向左偏移以創造重疊效果
            for (int j = i; j > 0; j--)
            {
                CardItem lastCardItem = handCard[j - 1].MyCardItem;
                RectTransform lastHandCardRect = lastCardItem.GetComponent<RectTransform>();
                lastHandCardRect.DOAnchorPosX(currentPosX - (i - j + 1) * cardXSpacing, moveTime);
                lastHandCardRect.DOAnchorPosY(lastCardItem.CurrentPos.y, moveTime);
                lastHandCardRect.DORotateQuaternion(Quaternion.Euler(0, 0, lastCardItem.CurrentAngle), moveTime);
            }
            currentPosX += cardXSpacing / 2; // 更新下一張卡片的起始位置
            // 設定卡片旋轉角度
            cardBag.RemoveAt(0);
            EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
        }
        yield return new WaitForSecondsRealtime(moveTime);
        HandCardRaycast();
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Attack);
    }
    private void HandCardRaycast()
    {
        List<CardData> handCard = DataManager.Instance.HandCard;
        for (int i = 0; i < handCard.Count; i++)
        {
            handCard[i].MyCardItem.CardCollision.raycastTarget = true;
        }
    }
    private void DrawnAllCards(int count)
    {
        List<CardData> usedCardBag = DataManager.Instance.UsedCardBag;
        List<CardData> cardBag = DataManager.Instance.CardBag;
        for (int j = 0; j < usedCardBag.Count; j++)
        {
            cardBag.Add(usedCardBag[j]);
        }
        for (int j = 0; j < cardBag.Count; j++)
        {
            CardItem cardItem = cardBag[j].MyCardItem;
            cardItem.transform.SetParent(transform);
            cardItem.GetComponent<RectTransform>().anchoredPosition = transform.position;
        }
        DataManager.Instance.UsedCardBag.Clear();
        BattleManager.Instance.Shuffle();
        CalculatePositionAngle(count, cardBag);
    }
    private void AdjustCard(int count)
    {
        List<CardData> handCard = DataManager.Instance.HandCard;
        CalculatePositionAngle(count, DataManager.Instance.HandCard);
        for (int i = 0; i < handCard.Count; i++)
        {
            CardItem cardItem = handCard[i].MyCardItem;
            RectTransform handCardRect = cardItem.GetComponent<RectTransform>();
            handCardRect.DOAnchorPos(cardItem.CurrentPos, moveTime);
            handCardRect.DORotateQuaternion(Quaternion.Euler(0, 0, cardItem.CurrentAngle), moveTime);
        }
    }
    private IEnumerator HideAllCards()
    {
        List<CardData> usedCardBag = DataManager.Instance.UsedCardBag;
        for (int i = 0; i < usedCardBag.Count; i++)
        {
            StartCoroutine(UIManager.Instance.FadeIn(usedCardBag[i].MyCardItem.GetComponent<CanvasGroup>(), moveTime / 2, false));
            yield return null;
        }
        yield return new WaitForSecondsRealtime(moveTime);
        for (int i = 0; i < usedCardBag.Count; i++)
        {
            usedCardBag[i].MyCardItem.gameObject.SetActive(false);
            yield return null;
        }
    }
    private void EventDrawCard(params object[] args)
    {
        StartCoroutine(DrawCard((int)args[0]));
    }
    private void EventUseCard(params object[] args)
    {
        currentPosX -= cardXSpacing / 2;
        CardItem cardItem = (CardItem)args[0];
        cardItem.transform.SetParent(usedCardTrans);
        AdjustCard(DataManager.Instance.HandCard.Count);
    }
    private void EventBattleInitial(params object[] args)
    {
        CreateCard();
    }
    private void EventPlayerTurn(params object[] args)
    {
        currentPosX = startPosition.x;
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.DrawCard);
        EventManager.Instance.DispatchEvent(EventDefinition.eventDrawCard, BattleManager.Instance.CurrentDrawCardCount);
        //StartCoroutine(DrawCard(BattleManager.Instance.CurrentDrawCardCount));
    }
    private void EventEnemyTurn(params object[] args)
    {
        List<CardData> freezeCardList = new List<CardData>();
        List<CardData> handCard = DataManager.Instance.HandCard;
        for (int i = 0; i < handCard.Count; i++)
        {
            if (handCard[i].CardFreeze)
            {
                freezeCardList.Add(handCard[i]);
                continue;
            }
            CardItem cardItem = handCard[i].MyCardItem;
            cardItem.CantMove = true;
            cardItem.transform.SetParent(usedCardTrans);
            cardItem.GetComponent<RectTransform>().DOAnchorPos(usedCardTrans.GetComponent<RectTransform>().anchoredPosition, moveTime);
            DataManager.Instance.UsedCardBag.Add(handCard[i]);
        }
        handCard.Clear();
        handCard = freezeCardList;
        AdjustCard(handCard.Count);
        StartCoroutine(HideAllCards());
    }
    private void EventBattleWin(params object[] args)
    {
        List<CardData> handCard = DataManager.Instance.HandCard;
        List<CardData> useCardBag = DataManager.Instance.UsedCardBag;
        List<CardData> removeCardBag = DataManager.Instance.RemoveCardBag;
        List<CardData> cardBag = DataManager.Instance.CardBag;
        for (int i = 0; i < handCard.Count; i++)
        {
            if (handCard[i].CardType == "詛咒")
            {
                continue;
            }
            cardBag.Add(handCard[i]);
        }
        for (int j = 0; j < useCardBag.Count; j++)
        {
            if (useCardBag[j].CardType == "詛咒")
            {
                continue;
            }
            cardBag.Add(useCardBag[j]);
        }
        for (int j = 0; j < removeCardBag.Count; j++)
        {
            if (removeCardBag[j].CardType == "詛咒")
            {
                continue;
            }
            cardBag.Add(removeCardBag[j]);
        }
        for (int j = 0; j < cardBag.Count; j++)
        {
            Destroy(cardBag[j].MyCardItem.gameObject);
        }
        removeCardBag.Clear();
        useCardBag.Clear();
        handCard.Clear();
    }
}
