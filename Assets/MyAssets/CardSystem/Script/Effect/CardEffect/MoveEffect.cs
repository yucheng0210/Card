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
        int playerOnceMoveConsume = BattleManager.Instance.PlayerOnceMoveConsume;
        int canMoveCount = value / playerOnceMoveConsume;
        emptyPlaceList = BattleManager.Instance.GetActionRangeTypeList(fromLocation, canMoveCount, BattleManager.CheckEmptyType.Move, BattleManager.ActionRangeType.Default);
        UIManager.Instance.ChangeCheckerboardColor(emptyPlaceList, true);
        for (int i = 0; i < emptyPlaceList.Count; i++)
        {
            int avoidClosure = i;
            int checkerboardPoint = BattleManager.Instance.GetCheckerboardPoint(emptyPlaceList[i]);
            int moveCount = BattleManager.Instance.GetRoute(fromLocation, emptyPlaceList[i], BattleManager.CheckEmptyType.Move).Count * playerOnceMoveConsume;
            RectTransform emptyPlace = BattleManager.Instance.CheckerboardTrans.GetChild(checkerboardPoint).GetComponent<RectTransform>();
            UnityEngine.Events.UnityAction moveAction = () => Move(emptyPlaceList[avoidClosure], moveCount);
            emptyPlace.GetComponent<Button>().onClick.AddListener(moveAction);
            removeList.Add(moveAction);
        }
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.UsingEffect);
    }
    private void Move(string location, int moveCount)
    {
        for (int i = 0; i < emptyPlaceList.Count; i++)
        {
            int checkerboardPoint = BattleManager.Instance.GetCheckerboardPoint(emptyPlaceList[i]);
            RectTransform emptyPlace = BattleManager.Instance.CheckerboardTrans.GetChild(checkerboardPoint).GetComponent<RectTransform>();
            emptyPlace.GetComponent<Button>().onClick.RemoveListener(removeList[i]);
        }
        BattleManager.Instance.PlayerMoveCount -= moveCount;
        PlayerMoveAction(BattleManager.Instance.CurrentLocationID, location);
        BattleManager.Instance.CurrentLocationID = location;
        UIManager.Instance.ClearMoveClue(true);
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
            EventManager.Instance.DispatchEvent(EventDefinition.eventMove);
            BattleManager.Instance.SwitchHandCardRaycast();
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
