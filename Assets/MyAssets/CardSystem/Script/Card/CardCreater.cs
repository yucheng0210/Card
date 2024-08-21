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

    [SerializeField]
    private GameObject roundTip;
    [SerializeField]
    private Sprite playerRound;
    [SerializeField]
    private Sprite enemyRound;
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
        // 檢查卡片數量是否為奇數
        CalculatePositionAngle(maxCardCount);
        // 逐個處理每張手牌卡片
        for (int i = handCard.Count; i < maxCardCount; i++)
        {
            // 根據抽卡數量將卡片添加到手牌中
            handCard.Add(BattleManager.Instance.CardItemList[0]);
            yield return new WaitForSecondsRealtime(coolDown); // 等待冷卻時間
            RectTransform handCardRect = handCard[i].GetComponent<RectTransform>();
            handCard[i].transform.SetParent(handCardTrans); // 將卡片設定為手牌的子物件
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
        BattleManager.Instance.CardPositionList.Clear();
        BattleManager.Instance.CardAngleList.Clear();
        currentPosX -= cardXSpacing / 2;
        CardItem cardItem = (CardItem)args[0];
        cardItem.transform.SetParent(usedCardTrans);
        AdjustCard();
    }

    private void EventBattleInitial(params object[] args)
    {
        roundTip.GetComponent<Image>().sprite = playerRound;
        CreateCard();
    }

    private void EventPlayerTurn(params object[] args)
    {
        currentPosX = startPosition.x;
        roundTip.GetComponent<Image>().sprite = playerRound;
        StartCoroutine(PlayerDrawCard());
    }


    private IEnumerator PlayerDrawCard()
    {
        yield return UIManager.Instance.FadeOutIn(roundTip.GetComponent<CanvasGroup>(), 0.5f, 1, false); // 執行 UI 淡入淡出效果
        StartCoroutine(DrawCard(DataManager.Instance.PlayerList[DataManager.Instance.PlayerID].DefaultDrawCardCout));
    }

    private void AdjustCard()
    {
        CalculatePositionAngle(DataManager.Instance.HandCard.Count);
        List<CardItem> handCard = DataManager.Instance.HandCard;
        for (int i = 0; i < handCard.Count; i++)
        {
            RectTransform handCardRect = handCard[i].GetComponent<RectTransform>();
            handCardRect.DOAnchorPos(BattleManager.Instance.CardPositionList[i], moveTime);
            handCardRect.DORotateQuaternion(Quaternion.Euler(0, 0, BattleManager.Instance.CardAngleList[i]), moveTime);
        }
    }

    private IEnumerator EnemyAttack()
    {
        Dictionary<string, EnemyData> newCurrentEnemyList = new();
        List<string> movedLocationList = new();
        yield return UIManager.Instance.FadeOutIn(roundTip.GetComponent<CanvasGroup>(), 0.5f, 1, false);
        yield return new WaitForSecondsRealtime(1);
        for (int i = 0; i < BattleManager.Instance.CurrentEnemyList.Count; i++)
        {
            string location = BattleManager.Instance.CurrentEnemyList.ElementAt(i).Key;
            string newLocation = location;
            int locationX = BattleManager.Instance.ConvertNormalPos(location)[0];
            int playerLocationX = BattleManager.Instance.ConvertNormalPos(BattleManager.Instance.CurrentLocationID)[0];
            EnemyData enemyData = BattleManager.Instance.CurrentEnemyList[location];
            int stepCount = enemyData.StepCount;
            RectTransform enemyTrans = enemyData.EnemyTrans;
            List<string> emptyPlaceList = BattleManager.Instance.GetEmptyPlace(location, stepCount, BattleManager.CheckEmptyType.Move);
            Enemy enemy = enemyTrans.GetComponent<Enemy>();
            PlayerData playerData = DataManager.Instance.PlayerList[DataManager.Instance.PlayerID];
            for (int j = 0; j < movedLocationList.Count; j++)//因為不是立即更新棋盤的空白位置
            {
                if (emptyPlaceList.Contains(movedLocationList[j]))
                    emptyPlaceList.Remove(movedLocationList[j]);
            }
            yield return new WaitForSecondsRealtime(0.5f);
            if (playerLocationX >= locationX)
                enemy.EnemyImage.transform.localRotation = Quaternion.Euler(0, 0, 0);
            else
                enemy.EnemyImage.transform.localRotation = Quaternion.Euler(0, 180, 0);
            switch (enemy.MyAttackType)
            {
                case Enemy.AttackType.Move:
                    float minDistance = 99;
                    int[] minPoint = new int[2];
                    for (int j = 0; j < emptyPlaceList.Count; j++)
                    {
                        int[] targetPoint = BattleManager.Instance.ConvertNormalPos(emptyPlaceList[j]);
                        float targetDistance = BattleManager.Instance.GetDistance(emptyPlaceList[j]);
                        if (targetDistance < minDistance)
                        {
                            minDistance = targetDistance;
                            minPoint = targetPoint;
                        }
                    }
                    newLocation = BattleManager.Instance.ConvertCheckerboardPos(minPoint[0], minPoint[1]);
                    int childCount = BattleManager.Instance.GetCheckerboardPoint(newLocation);
                    RectTransform emptyPlace = BattleManager.Instance.CheckerboardTrans.GetChild(childCount).GetComponent<RectTransform>();
                    enemyTrans.DOAnchorPos(emptyPlace.localPosition, 0.5f);
                    newCurrentEnemyList.Add(newLocation, enemyData);
                    movedLocationList.Add(newLocation);
                    enemyData.CurrentAttackOrder--;
                    enemy.MyAnimator.SetBool("isRunning", true);
                    break;
                case Enemy.AttackType.Attack:
                    enemy.MyAnimator.SetTrigger("isAttacking");
                    yield return new WaitForSecondsRealtime(0.25f);
                    BattleManager.Instance.TakeDamage(playerData, enemyData.CurrentAttack, BattleManager.Instance.CurrentLocationID);
                    newCurrentEnemyList.Add(newLocation, BattleManager.Instance.CurrentEnemyList[location]);
                    break;
                case Enemy.AttackType.Shield:
                    BattleManager.Instance.GetShield(enemyData, enemyData.CurrentAttack / 2);
                    newCurrentEnemyList.Add(newLocation, BattleManager.Instance.CurrentEnemyList[location]);
                    break;
                case Enemy.AttackType.Effect:
                    EffectFactory.Instance.CreateEffect(enemyData.AttackOrderStrs[enemyData.CurrentAttackOrder].ToString()).ApplyEffect(1, "Player");
                    newCurrentEnemyList.Add(newLocation, BattleManager.Instance.CurrentEnemyList[location]);
                    break;
            }
            yield return new WaitForSecondsRealtime(0.5f);
            enemy.MyAnimator.SetBool("isRunning", false);
            if (enemyData.CurrentAttackOrder >= enemyData.AttackOrderStrs.Length - 1)
                enemyData.CurrentAttackOrder = 0;
            else
                enemyData.CurrentAttackOrder++;

            EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
            yield return new WaitForSecondsRealtime(1);
        }
        BattleManager.Instance.CurrentEnemyList.Clear();
        for (int i = 0; i < newCurrentEnemyList.Count; i++)
        {
            BattleManager.Instance.CurrentEnemyList.Add(newCurrentEnemyList.ElementAt(i).Key, newCurrentEnemyList.ElementAt(i).Value);
            Enemy newEnemy = BattleManager.Instance.CurrentEnemyList[newCurrentEnemyList.ElementAt(i).Key].EnemyTrans.GetComponent<Enemy>();
            newEnemy.EnemyLocation = newCurrentEnemyList.ElementAt(i).Key;
        }
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Player);
    }
    private void EventEnemyTurn(params object[] args)
    {
        List<CardItem> freezeCardList = new List<CardItem>();
        for (int i = 0; i < DataManager.Instance.HandCard.Count; i++)
        {
            if (DataManager.Instance.CardList[DataManager.Instance.HandCard[i].CardID].CardFreeze)
            {
                freezeCardList.Add(DataManager.Instance.HandCard[i]);
                continue;
            }
            DataManager.Instance.HandCard[i].gameObject.SetActive(true);
            DataManager.Instance.HandCard[i].CantMove = true;
            DataManager.Instance.HandCard[i].transform.SetParent(usedCardTrans);
            DataManager.Instance.HandCard[i].GetComponent<RectTransform>().DOAnchorPos(usedCardTrans.GetComponent<RectTransform>().anchoredPosition, moveTime);
            DataManager.Instance.UsedCardBag.Add(DataManager.Instance.HandCard[i]);
        }
        DataManager.Instance.HandCard.Clear();
        DataManager.Instance.HandCard = freezeCardList;
        BattleManager.Instance.CardPositionList.Clear();
        BattleManager.Instance.CardAngleList.Clear();
        AdjustCard();
        StartCoroutine(HideAllCards());
        roundTip.GetComponent<Image>().sprite = enemyRound;
        StartCoroutine(EnemyAttack());
    }

    private void EventBattleWin(params object[] args)
    {
        for (int i = 0; i < DataManager.Instance.HandCard.Count; i++)
        {
            BattleManager.Instance.CardItemList.Add(DataManager.Instance.HandCard[i]);
            int index = DataManager.Instance.HandCard[i].CardID;
            if (DataManager.Instance.CardList[index].CardType == "詛咒")
                continue;
            DataManager.Instance.CardBag.Add(DataManager.Instance.CardList[index]);
        }
        for (int j = 0; j < DataManager.Instance.UsedCardBag.Count; j++)
        {
            BattleManager.Instance.CardItemList.Add(DataManager.Instance.UsedCardBag[j]);
            int index = DataManager.Instance.UsedCardBag[j].CardID;
            if (DataManager.Instance.CardList[index].CardType == "詛咒")
                continue;
            DataManager.Instance.CardBag.Add(DataManager.Instance.CardList[index]);
        }
        for (int j = 0; j < DataManager.Instance.RemoveCardBag.Count; j++)
        {
            BattleManager.Instance.CardItemList.Add(DataManager.Instance.RemoveCardBag[j]);
            int index = DataManager.Instance.RemoveCardBag[j].CardID;
            if (DataManager.Instance.CardList[index].CardType == "詛咒")
                continue;
            DataManager.Instance.CardBag.Add(DataManager.Instance.CardList[index]);
        }
        for (int j = 0; j < BattleManager.Instance.CardItemList.Count; j++)
        {
            Destroy(BattleManager.Instance.CardItemList[j].gameObject);
        }
        BattleManager.Instance.CardAngleList.Clear();
        BattleManager.Instance.CardPositionList.Clear();
        DataManager.Instance.HandCard.Clear();
        DataManager.Instance.UsedCardBag.Clear();
        DataManager.Instance.RemoveCardBag.Clear();
        BattleManager.Instance.CardItemList.Clear();
    }
}
