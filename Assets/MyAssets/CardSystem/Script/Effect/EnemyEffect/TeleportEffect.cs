using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class TeleportEffect : IEffect
{
    private CharacterData characterData;
    public void ApplyEffect(int value, string target)
    {
        characterData = BattleManager.Instance.IdentifyCharacter(target);
        float minDistance = BattleManager.Instance.CalculateDistance(target, BattleManager.Instance.CurrentLocationID);
        List<string> teleportList = new();
        for (int i = 0; i < BattleManager.Instance.CheckerboardList.Count; i++)
        {
            string key = BattleManager.Instance.CheckerboardList.ElementAt(i).Key;
            float currentDistance = BattleManager.Instance.CalculateDistance(key, BattleManager.Instance.CurrentLocationID);
            if (BattleManager.Instance.CheckPlaceEmpty(target, BattleManager.CheckEmptyType.Move) && currentDistance >= minDistance && target != key)
            {
                teleportList.Add(key);
            }
        }
        int randomIndex = Random.Range(0, teleportList.Count);
        BattleManager.Instance.CurrentEnemyList.Remove(target);
        BattleManager.Instance.CurrentEnemyList.Add(teleportList[randomIndex], (EnemyData)characterData);
        int childCount = BattleManager.Instance.GetCheckerboardPoint(teleportList[randomIndex]);
        RectTransform emptyPlace = BattleManager.Instance.CheckerboardTrans.GetChild(childCount).GetComponent<RectTransform>();
        BattleManager.Instance.CurrentEnemyList[teleportList[randomIndex]].EnemyTrans.DOAnchorPos(emptyPlace.localPosition, 0);
    }
    public string SetTitleText()
    {
        return "瞬閃";
    }

    public string SetDescriptionText()
    {
        return "隨機朝遠離敵人的方向瞬移。";
    }

}
