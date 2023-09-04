using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MoveEffect : IEffect
{
    public void ApplyEffect(int value, string target)
    {
        List<string> emptyPlaceList = BattleManager.Instance.GetEmptyPlace(BattleManager.Instance.CurrentLocationID, value);
        for (int i = 0; i < emptyPlaceList.Count; i++)
        {
            RectTransform emptyPlace = BattleManager.Instance.CheckerboardTrans
            .GetChild(BattleManager.Instance.GetCheckerboardPoint(emptyPlaceList[i])).GetComponent<RectTransform>();
            int avoidClosure = i;
            emptyPlace.GetComponent<Image>().color = Color.green;
            emptyPlace.GetComponent<Button>().onClick.AddListener(() =>
            Move(emptyPlace.localPosition, emptyPlaceList[avoidClosure]));
            Debug.Log("玩家可行走位置：" + emptyPlaceList[i]);
        }
    }
    private void Move(Vector2 destination, string loactionID)
    {
        BattleManager.Instance.CurrentLocationID = loactionID;
        RectTransform checkerboardTrans = BattleManager.Instance.CheckerboardTrans;
        BattleManager.Instance.PlayerTrans.DOAnchorPos(destination, 0.5f);
        BattleManager.Instance.RefreshEnemyAlert();
        for (int i = 0; i < checkerboardTrans.childCount; i++)
        {
            checkerboardTrans.GetChild(i).GetComponent<Image>().color = Color.white;
            checkerboardTrans.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }
}
