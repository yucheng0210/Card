using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MoveEffect : IEffect
{
    private List<UnityEngine.Events.UnityAction> removeList = new();
    private List<string> emptyPlaceList;

    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        emptyPlaceList = BattleManager.Instance.GetAcitonRangeTypeList(fromLocation, value, BattleManager.CheckEmptyType.Move, BattleManager.ActionRangeType.Default);
        UIManager.Instance.ChangeCheckerboardColor(emptyPlaceList, true);
        for (int i = 0; i < emptyPlaceList.Count; i++)
        {
            int avoidClosure = i;
            int checkerboardPoint = BattleManager.Instance.GetCheckerboardPoint(emptyPlaceList[i]);
            int playerOnceMoveConsume = BattleManager.Instance.PlayerOnceMoveConsume;
            int moveCount = BattleManager.Instance.GetRoute(fromLocation, emptyPlaceList[i], BattleManager.CheckEmptyType.Move).Count * playerOnceMoveConsume;
            RectTransform emptyPlace = BattleManager.Instance.CheckerboardTrans.GetChild(checkerboardPoint).GetComponent<RectTransform>();
            UnityEngine.Events.UnityAction moveAction = () => Move(emptyPlace.localPosition, emptyPlaceList[avoidClosure], moveCount);
            emptyPlace.GetComponent<Button>().onClick.AddListener(moveAction);
            removeList.Add(moveAction);
        }
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.UsingEffect);
    }
    private void Move(Vector3 destination, string locationID, int moveCount)
    {
        for (int i = 0; i < emptyPlaceList.Count; i++)
        {
            int checkerboardPoint = BattleManager.Instance.GetCheckerboardPoint(emptyPlaceList[i]);
            RectTransform emptyPlace = BattleManager.Instance.CheckerboardTrans.GetChild(checkerboardPoint).GetComponent<RectTransform>();
            emptyPlace.GetComponent<Button>().onClick.RemoveListener(removeList[i]);
        }
        BattleManager.Instance.PlayerMoveCount -= moveCount;
        PlayerMoveAction(BattleManager.Instance.CurrentLocationID, locationID);
        BattleManager.Instance.CurrentLocationID = locationID;
        EventManager.Instance.DispatchEvent(EventDefinition.eventMove);
    }
    private void PlayerMoveAction(string fromLocation, string toLocation)
    {
        List<string> routeList = BattleManager.Instance.GetRoute(fromLocation, toLocation, BattleManager.CheckEmptyType.Move);
        Sequence sequence = DOTween.Sequence();
        for (int k = 0; k < routeList.Count; k++)
        {
            int childCount = BattleManager.Instance.GetCheckerboardPoint(routeList[k]);
            RectTransform emptyPlace = BattleManager.Instance.CheckerboardTrans.GetChild(childCount).GetComponent<RectTransform>();
            sequence.Append(BattleManager.Instance.PlayerTrans.DOAnchorPos(emptyPlace.localPosition, 0.5f));
        }
        sequence.OnComplete(() =>
        {
            BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Attack);
            EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
        }
        );
        sequence.Play();
    }
    public string SetTitleText()
    {
        throw new NotImplementedException();
    }
    public string SetDescriptionText()
    {
        throw new NotImplementedException();
    }

}