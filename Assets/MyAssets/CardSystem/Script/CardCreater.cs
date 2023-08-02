using System.Data;
using System.Net.Mime;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CardCreater : MonoBehaviour
{
    [SerializeField]
    private Vector2 startPosition = new Vector2(878, -20);

    [SerializeField]
    private CardItem cardPrefab;

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

    [SerializeField]
    private GameObject roundTip;
    private float currentPosX;

    private void Start()
    {
        //StartCoroutine(DrawCard());
        EventManager.Instance.AddEventRegister(EventDefinition.eventUseCard, EventUseCard);
        EventManager.Instance.AddEventRegister(EventDefinition.eventEnemyTurn, EventEnemyTurn);
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
        EventManager.Instance.AddEventRegister(EventDefinition.eventBattleWin, EventBattleWin);
        EventManager.Instance.AddEventRegister(
            EventDefinition.eventBattleInitial,
            EventBattleInitial
        );
        EventManager.Instance.AddEventRegister(EventDefinition.eventDrawCard, EventDrawCard);
    }

    private void CreateCard()
    {
        List<CardData> cardBag = DataManager.Instance.CardBag;
        BattleManager.Instance.Shuffle(); // 在這之前將卡片順序洗牌
        for (int i = 0; i < cardBag.Count; i++)
        {
            CardItem cardItem = Instantiate(cardPrefab, transform);
            cardItem.CardID = cardBag[i].CardID;
            cardItem.CardName.text = cardBag[i].CardName;
            cardItem.CardDescription.text = cardBag[i].CardDescription;
            cardItem.CardCost.text = cardBag[i].CardCost.ToString();
            cardItem.gameObject.SetActive(false);
            BattleManager.Instance.CardItemList.Add(cardItem);
        }
    }

    private void CalculatePositionAngle(int cardCount)
    {
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
        BattleManager.Instance.CardPositionList.Clear();
        BattleManager.Instance.CardAngleList.Clear();
        List<CardItem> handCard = DataManager.Instance.HandCard;
        int maxCardCount = handCard.Count + addCardCount;
        int odd = maxCardCount % 2 != 0 ? 0 : 1;
        // 檢查卡片數量是否為奇數
        CalculatePositionAngle(maxCardCount);
        // 逐個處理每張手牌卡片
        for (int i = handCard.Count; i < maxCardCount; i++)
        {
            // 根據抽卡數量將卡片添加到手牌中
            handCard.Add(BattleManager.Instance.CardItemList[0]);
            yield return new WaitForSecondsRealtime(coolDown); // 等待冷卻時間
            handCard[i].transform.SetParent(handCardTrans); // 將卡片設定為手牌的子物件
            handCard[i].gameObject.SetActive(true); // 啟用卡片物件
            StartCoroutine(
                UIManager.Instance.FadeOut(handCard[i].GetComponent<CanvasGroup>(), moveTime)
            );
            handCard[i].GetComponent<RectTransform>().DOAnchorPosX(currentPosX, moveTime); // 使用 DOTween 動畫設定卡片位置
            handCard[i]
                .GetComponent<RectTransform>()
                .DOAnchorPosY(BattleManager.Instance.CardPositionList[i].y, moveTime);
            handCard[i]
                .GetComponent<RectTransform>()
                .DORotateQuaternion(
                    Quaternion.Euler(0, 0, BattleManager.Instance.CardAngleList[i]),
                    moveTime
                );

            // 將前面的卡片向左偏移以創造重疊效果
            for (int j = i; j > 0; j--)
            {
                handCard[j - 1]
                    .GetComponent<RectTransform>()
                    .DOAnchorPosX(currentPosX - (i - j + 1) * cardXSpacing, moveTime);
                handCard[j - 1]
                    .GetComponent<RectTransform>()
                    .DOAnchorPosY(BattleManager.Instance.CardPositionList[j - 1].y, moveTime);
                handCard[j - 1]
                    .GetComponent<RectTransform>()
                    .DORotateQuaternion(
                        Quaternion.Euler(0, 0, BattleManager.Instance.CardAngleList[j - 1]),
                        moveTime
                    );
            }
            currentPosX += cardXSpacing / 2; // 更新下一張卡片的起始位置
            // 設定卡片旋轉角度
            DataManager.Instance.CardBag.RemoveAt(0);
            BattleManager.Instance.CardItemList.RemoveAt(0);
            EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
            if (DataManager.Instance.CardBag.Count == 0)
                DrawnAllCards();
        }
        yield return new WaitForSecondsRealtime(moveTime);
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Attack);
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
            BattleManager.Instance.CardItemList[j].GetComponent<RectTransform>().anchoredPosition =
                transform.position;
        }
        DataManager.Instance.UsedCardBag.Clear();
        BattleManager.Instance.Shuffle();
    }

    private IEnumerator HideAllCards()
    {
        for (int i = 0; i < DataManager.Instance.UsedCardBag.Count; i++)
        {
            StartCoroutine(
                UIManager.Instance.FadeIn(
                    DataManager.Instance.UsedCardBag[i].GetComponent<CanvasGroup>(),
                    moveTime / 2,
                    false
                )
            );
            yield return null;
        }
        yield return new WaitForSecondsRealtime(moveTime);
        for (int i = 0; i < DataManager.Instance.UsedCardBag.Count; i++)
        {
            DataManager.Instance.UsedCardBag[i].gameObject.SetActive(false);
            yield return null;
        }
    }

    private IEnumerator EnemyAttack()
    {
        yield return (
            UIManager.Instance.FadeOutIn(roundTip.GetComponent<CanvasGroup>(), 0.5f, 1, false)
        );
        yield return new WaitForSecondsRealtime(1);
        for (int i = 0; i < BattleManager.Instance.CurrentEnemyList.Count; i++)
        {
            BattleManager.Instance.TakeDamage(
                DataManager.Instance.PlayerList[DataManager.Instance.PlayerID],
                BattleManager.Instance.CurrentEnemyList[i].CurrentAttack
            );
            EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
            yield return new WaitForSecondsRealtime(1);
        }
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Player);
    }

    private IEnumerator PlayerDrawCard()
    {
        yield return (
            UIManager.Instance.FadeOutIn(roundTip.GetComponent<CanvasGroup>(), 0.5f, 1, false)
        ); // 執行 UI 淡入淡出效果
        StartCoroutine(
            DrawCard(
                DataManager.Instance.PlayerList[DataManager.Instance.PlayerID].DefaultDrawCardCout
            )
        );
    }

    private void AdjustCard()
    {
        List<CardItem> handCard = DataManager.Instance.HandCard;
        for (int i = 0; i < handCard.Count; i++)
        {
            handCard[i]
                .GetComponent<RectTransform>()
                .DOAnchorPos(BattleManager.Instance.CardPositionList[i], moveTime);
            handCard[i]
                .GetComponent<RectTransform>()
                .DORotateQuaternion(
                    Quaternion.Euler(0, 0, BattleManager.Instance.CardAngleList[i]),
                    moveTime
                );
        }
    }

    private void EventDrawCard(params object[] args)
    {
        StartCoroutine(DrawCard((int)(args[0])));
    }

    private void EventUseCard(params object[] args)
    {
        BattleManager.Instance.CardPositionList.Clear();
        BattleManager.Instance.CardAngleList.Clear();
        currentPosX -= cardXSpacing / 2;
        CardItem cardItem = (CardItem)args[0];
        DataManager.Instance.UsedCardBag.Add(cardItem);
        cardItem.transform.SetParent(usedCardTrans);
        CalculatePositionAngle(DataManager.Instance.HandCard.Count);
        AdjustCard();
    }

    private void EventBattleInitial(params object[] args)
    {
        roundTip.GetComponentInChildren<Text>().text = "戰鬥開始";
        CreateCard();
    }

    private void EventPlayerTurn(params object[] args)
    {
        currentPosX = startPosition.x;
        roundTip.GetComponentInChildren<Text>().text = "我方回合";
        StartCoroutine(PlayerDrawCard());
    }

    private void EventEnemyTurn(params object[] args)
    {
        for (int i = 0; i < DataManager.Instance.HandCard.Count; i++)
        {
            DataManager.Instance.HandCard[i].gameObject.SetActive(true);
            DataManager.Instance.HandCard[i].transform.SetParent(usedCardTrans);
            DataManager.Instance.HandCard[i]
                .GetComponent<RectTransform>()
                .DOAnchorPos(
                    usedCardTrans.GetComponent<RectTransform>().anchoredPosition,
                    moveTime
                );
            DataManager.Instance.UsedCardBag.Add(DataManager.Instance.HandCard[i]);
        }
        DataManager.Instance.HandCard.Clear();
        BattleManager.Instance.CardPositionList.Clear();
        BattleManager.Instance.CardAngleList.Clear();
        StartCoroutine(HideAllCards());
        roundTip.GetComponentInChildren<Text>().text = "敵方回合";
        StartCoroutine(EnemyAttack());
    }

    private void EventBattleWin(params object[] args)
    {
        for (int i = 0; i < DataManager.Instance.HandCard.Count; i++)
        {
            BattleManager.Instance.CardItemList.Add(DataManager.Instance.HandCard[i]);
            int index = DataManager.Instance.HandCard[i].CardID;
            DataManager.Instance.CardBag.Add(DataManager.Instance.CardList[index]);
        }
        for (int j = 0; j < DataManager.Instance.UsedCardBag.Count; j++)
        {
            BattleManager.Instance.CardItemList.Add(DataManager.Instance.UsedCardBag[j]);
            int index = DataManager.Instance.UsedCardBag[j].CardID;
            DataManager.Instance.CardBag.Add(DataManager.Instance.CardList[index]);
        }
        for (int j = 0; j < BattleManager.Instance.CardItemList.Count; j++)
        {
            Destroy(BattleManager.Instance.CardItemList[j].gameObject);
        }
        BattleManager.Instance.CardAngleList.Clear();
        BattleManager.Instance.CardPositionList.Clear();
        BattleManager.Instance.CurrentEnemyList.Clear();
        BattleManager.Instance.CurrentAbilityList.Clear();
        DataManager.Instance.HandCard.Clear();
        DataManager.Instance.UsedCardBag.Clear();
        BattleManager.Instance.CardItemList.Clear();
    }
}
