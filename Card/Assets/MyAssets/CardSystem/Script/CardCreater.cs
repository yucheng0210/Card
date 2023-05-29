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
    private float startPosY;

    [SerializeField]
    private CardItem cardPrefab;

    [SerializeField]
    private Transform handCardTrans;

    [SerializeField]
    private Transform usedCardTrans;

    [SerializeField]
    private int drawCardCount;

    [SerializeField]
    private float coolDown;

    [SerializeField]
    private float cardXSpacing;

    [SerializeField]
    private float cardYSpacing;

    [SerializeField]
    private float reduceValue;

    [SerializeField]
    private float minCardAngle;

    [SerializeField]
    private float moveTime;

    [SerializeField]
    private GameObject roundTip;

    [SerializeField]
    private int cardID;

    [SerializeField]
    private List<CardItem> cardItemList = new List<CardItem>();

    private void Start()
    {
        CreateCard();
        //StartCoroutine(DrawCard());
        EventManager.Instance.AddEventRegister(
            EventDefinition.eventUseCard,
            EventCardAdjustmentPosition
        );
        EventManager.Instance.AddEventRegister(EventDefinition.eventEnemyTurn, EventEnemyTurn);
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
    }

    private void CreateCard()
    {
        List<CardData> cardBag = DataManager.Instance.CardBag;
        for (int i = 0; i < cardBag.Count; i++)
        {
            CardItem cardItem = Instantiate(cardPrefab, transform);
            cardItem.CardIndex = cardBag[i].CardID;
            cardItem.CardName.text = cardBag[i].CardName;
            cardItem.CardDescription.text = cardBag[i].CardDescription;
            cardItem.CardCost.text = cardBag[i].CardCost.ToString();
            cardItem.gameObject.SetActive(false);
            cardItemList.Add(cardItem);
        }
        BattleManager.Instance.Shuffle(); // 在這之前將卡片順序洗牌
    }

    private void CalculatePositionAngle(int cardCount)
    {
        Vector2 startPosition = new Vector2(878, startPosY);
        int odd = cardCount % 2 != 0 ? 0 : 1;
        float startAngle = (cardCount / 2 - odd) * minCardAngle;
        for (int i = 0; i < cardCount; i++)
        {
            BattleManager.Instance.CardPositionList.Add(startPosition);
            BattleManager.Instance.CardAngleList.Add(startAngle);
            for (int j = i; j > 0; j--)
            {
                float adjustmentPosX = startPosition.x;
                adjustmentPosX -= (i - j + 1) * cardXSpacing * 2;
                BattleManager.Instance.CardPositionList[j - 1] = new Vector2(
                    adjustmentPosX,
                    BattleManager.Instance.CardPositionList[j - 1].y
                );
            }
            startPosition.x += cardXSpacing;
            if (odd != 0 && i == (cardCount / 2 - 1))
                continue;
            startAngle -= minCardAngle; // 更新下一張卡片的旋轉角度
            startPosition.y +=
                cardYSpacing
                * ((cardCount / 2 - odd - i) - (cardCount / 2 - odd <= i && odd == 0 ? 1 : 0)); // 更新下一張卡片的起始位置
        }
    }

    private IEnumerator DrawCard()
    {
        StartCoroutine(
            UIManager.Instance.FadeOutIn(roundTip.GetComponent<CanvasGroup>(), 0.5f, 1, false)
        ); // 執行 UI 淡入淡出效果
        yield return new WaitForSecondsRealtime(1.5f); // 等待 1.5 秒
        CalculatePositionAngle(drawCardCount);
        // 設定初始卡片位置
        Vector2 startPosition = new Vector2(878, startPosY);

        // 檢查卡片數量是否為奇數
        int odd = drawCardCount % 2 != 0 ? 0 : 1;

        // 計算初始旋轉角度
        float startAngle = (drawCardCount / 2 - odd) * minCardAngle;
        if (cardItemList.Count < drawCardCount)
        {
            for (int i = 0; i < DataManager.Instance.UsedCardBag.Count; i++)
            {
                cardItemList.Add(DataManager.Instance.UsedCardBag[i]);
                int index = DataManager.Instance.UsedCardBag[i].CardIndex;
                DataManager.Instance.CardBag.Add(DataManager.Instance.CardList[index]);
            }
            DataManager.Instance.UsedCardBag.Clear();
        }
        // 根據抽卡數量將卡片添加到手牌中
        BattleManager.Instance.AddHandCard(drawCardCount, cardItemList);
        cardID += drawCardCount;
        cardItemList.RemoveRange(0, drawCardCount);
        List<CardItem> handCard = DataManager.Instance.HandCard;

        // 逐個處理每張手牌卡片
        for (int i = 0; i < handCard.Count; i++)
        {
            yield return new WaitForSecondsRealtime(coolDown); // 等待冷卻時間

            handCard[i].transform.SetParent(handCardTrans); // 將卡片設定為手牌的子物件
            handCard[i].gameObject.SetActive(true); // 啟用卡片物件
            handCard[i].GetComponent<RectTransform>().DOAnchorPos(startPosition, moveTime); // 使用 DOTween 動畫設定卡片位置
            handCard[i].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, startAngle); // 設定卡片旋轉角度
            // 將前面的卡片向左偏移以創造重疊效果
            for (int j = i; j > 0; j--)
            {
                handCard[j - 1]
                    .GetComponent<RectTransform>()
                    .DOAnchorPosX(startPosition.x - (i - j + 1) * cardXSpacing * 2, moveTime);
            }
            startPosition.x += cardXSpacing; // 更新下一張卡片的起始位置
            if (odd != 0 && i == (drawCardCount / 2 - 1))
                continue;
            startAngle -= minCardAngle; // 更新下一張卡片的旋轉角度
            startPosition.y +=
                cardYSpacing
                * (
                    (drawCardCount / 2 - odd - i)
                    - (drawCardCount / 2 - odd <= i && odd == 0 ? 1 : 0)
                ); // 更新下一張卡片的起始位置
        }
    }

    private void HideAllCards()
    {
        for (int i = 0; i < DataManager.Instance.UsedCardBag.Count; i++)
        {
            DataManager.Instance.UsedCardBag[i].gameObject.SetActive(false);
        }
    }

    private IEnumerator EnemyAttack()
    {
        yield return (
            StartCoroutine(
                UIManager.Instance.FadeOutIn(roundTip.GetComponent<CanvasGroup>(), 0.5f, 1, false)
            )
        );
        yield return new WaitForSecondsRealtime(1);
        for (
            int i = 0;
            i < DataManager.Instance.LevelList[DataManager.Instance.LevelID].EnemyIDList.Count;
            i++
        )
        {
            int enemyID = DataManager.Instance.LevelList[DataManager.Instance.LevelID].EnemyIDList[
                i
            ].Item1;
            int randomAttack = UnityEngine.Random.Range(
                DataManager.Instance.EnemyList[enemyID].MinAttack,
                DataManager.Instance.EnemyList[enemyID].MaxAttack + 1
            );
            BattleManager.Instance.TakeDamage(
                DataManager.Instance.PlayerList[DataManager.Instance.PlayerID],
                randomAttack
            );
            yield return new WaitForSecondsRealtime(1);
        }
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Player);
    }

    private void EventCardAdjustmentPosition(params object[] args)
    {
        BattleManager.Instance.CardPositionList.Clear();
        BattleManager.Instance.CardAngleList.Clear();
        CardItem cardItem = (CardItem)args[0];
        DataManager.Instance.UsedCardBag.Add(cardItem);
        cardItem.transform.SetParent(usedCardTrans);
        cardItemList.Remove(cardItem);
        List<CardItem> handCard = DataManager.Instance.HandCard;
        CalculatePositionAngle(handCard.Count);
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

    private void EventPlayerTurn(params object[] args)
    {
        roundTip.GetComponentInChildren<Text>().text = "我方回合";
        StartCoroutine(DrawCard());
    }

    private void EventEnemyTurn(params object[] args)
    {
        for (int i = 0; i < DataManager.Instance.HandCard.Count; i++)
        {
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
        Invoke("HideAllCards", moveTime);
        roundTip.GetComponentInChildren<Text>().text = "敵方回合";
        StartCoroutine(EnemyAttack());
    }
}
