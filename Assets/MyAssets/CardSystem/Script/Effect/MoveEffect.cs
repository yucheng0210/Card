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
        List<string> emptyPlaceList = new();
        string[] pos = BattleManager.Instance.CurrentLocationID.Split(' ');
        int x = int.Parse(pos[0]);
        int y = int.Parse(pos[1]);
        emptyPlaceList.Add(BattleManager.Instance.ConvertCheckerboardPos(x, y + 1));
        emptyPlaceList.Add(BattleManager.Instance.ConvertCheckerboardPos(x, y - 1));
        emptyPlaceList.Add(BattleManager.Instance.ConvertCheckerboardPos(x + 1, y));
        emptyPlaceList.Add(BattleManager.Instance.ConvertCheckerboardPos(x + 1, y + 1));
        emptyPlaceList.Add(BattleManager.Instance.ConvertCheckerboardPos(x + 1, y - 1));
        emptyPlaceList.Add(BattleManager.Instance.ConvertCheckerboardPos(x - 1, y));
        emptyPlaceList.Add(BattleManager.Instance.ConvertCheckerboardPos(x - 1, y + 1));
        emptyPlaceList.Add(BattleManager.Instance.ConvertCheckerboardPos(x - 1, y - 1));
        for (int i = 0; i < emptyPlaceList.Count; i++)
        {
            if (BattleManager.Instance.CheckerboardList.ContainsKey(emptyPlaceList[i]))
            {
                if (BattleManager.Instance.CheckerboardList[emptyPlaceList[i]] == "Empty")
                {
                    Transform emptyPlace = BattleManager.Instance.CheckerboardTrans
                    .GetChild(BattleManager.Instance.GetCheckerboardPoint(emptyPlaceList[i]));
                    int avoidClosure = i;
                    emptyPlace.GetComponent<Image>().color = Color.green;
                    emptyPlace.GetComponent<Button>().onClick.AddListener(() => 
                    Move(emptyPlace.localPosition, emptyPlaceList[avoidClosure]));
                }
            }
        }
    }
    private void Move(Vector2 destination, string loactionID)
    {
        BattleManager.Instance.CurrentLocationID = loactionID;
        Transform checkerboardTrans = BattleManager.Instance.CheckerboardTrans;
        BattleManager.Instance.PlayerTrans.DOAnchorPos(destination, 1);
        for (int i = 0; i < checkerboardTrans.childCount; i++)
        {
            checkerboardTrans.GetChild(i).GetComponent<Image>().color = Color.white;
            checkerboardTrans.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }
}
