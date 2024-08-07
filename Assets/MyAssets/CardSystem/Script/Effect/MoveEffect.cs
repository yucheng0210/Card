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

    public void ApplyEffect(int value, string target)
    {
        emptyPlaceList = BattleManager.Instance.GetEmptyPlace(BattleManager.Instance.CurrentLocationID, value, BattleManager.CheckEmptyType.Move);
        Color color = new Color(0.2f, 0.8f, 0.16f, 1);
        UIManager.Instance.ChangeCheckerboardColor(color, BattleManager.Instance.CurrentLocationID, value, BattleManager.CheckEmptyType.Move);
        for (int i = 0; i < emptyPlaceList.Count; i++)
        {
            int avoidClosure = i;
            int checkerboardPoint = BattleManager.Instance.GetCheckerboardPoint(emptyPlaceList[i]);
            int moveCount = (int)BattleManager.Instance.GetDistance(emptyPlaceList[avoidClosure]) + 1;
            RectTransform emptyPlace = BattleManager.Instance.CheckerboardTrans.GetChild(checkerboardPoint).GetComponent<RectTransform>();
            UnityEngine.Events.UnityAction moveAction = () => Move(emptyPlace.localPosition, emptyPlaceList[avoidClosure], value, moveCount);
            emptyPlace.GetComponent<Button>().onClick.AddListener(moveAction);
            removeList.Add(moveAction);
        }
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.UsingEffect);
    }

    private void Move(Vector3 destination, string locationID, int value, int moveCount)
    {
        for (int i = 0; i < emptyPlaceList.Count; i++)
        {
            int checkerboardPoint = BattleManager.Instance.GetCheckerboardPoint(emptyPlaceList[i]);
            RectTransform emptyPlace = BattleManager.Instance.CheckerboardTrans.GetChild(checkerboardPoint).GetComponent<RectTransform>();
            emptyPlace.GetComponent<Button>().onClick.RemoveListener(removeList[i]);
        }
        BattleManager.Instance.PlayerMoveCount -= moveCount;
        UIManager.Instance.ClearCheckerboardColor(BattleManager.Instance.CurrentLocationID, value, BattleManager.CheckEmptyType.Move);
        BattleManager.Instance.CurrentLocationID = locationID;
        BattleManager.Instance.PlayerTrans.DOAnchorPos(destination, 0.5f);
        BattleManager.Instance.ChangeTurn(BattleManager.BattleType.Attack);
        EventManager.Instance.DispatchEvent(EventDefinition.eventMove);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    Sprite IEffect.SetIcon()
    {
        throw new NotImplementedException();
    }
}
