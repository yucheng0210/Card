using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class KnockBackEffect : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        CharacterData targetData;
        int[] attackerPos = BattleManager.Instance.ConvertNormalPos(fromLocation);
        int[] defenderPos = BattleManager.Instance.ConvertNormalPos(toLocation);
        Vector2 direction;
        Vector2Int destinationPoint;
        targetData = BattleManager.Instance.IdentifyCharacter(toLocation);
        direction = new Vector2(defenderPos[0] - attackerPos[0], defenderPos[1] - attackerPos[1]).normalized;
        destinationPoint = new Vector2Int(defenderPos[0] + (int)direction.x, defenderPos[1] + (int)direction.y);
        string destinationLocation = BattleManager.Instance.ConvertCheckerboardPos(destinationPoint.x, destinationPoint.y);
        if (!BattleManager.Instance.CheckerboardList.ContainsKey(destinationLocation))
        {
            destinationLocation = toLocation;
        }
        int checkerboardPoint = BattleManager.Instance.GetCheckerboardPoint(destinationLocation);
        Vector3 destination = BattleManager.Instance.CheckerboardTrans.GetChild(checkerboardPoint).GetComponent<RectTransform>().localPosition;
        if (toLocation == BattleManager.Instance.CurrentLocationID)
        {
            BattleManager.Instance.PlayerTrans.DOAnchorPos(destination, 0.2f);
            BattleManager.Instance.CurrentLocationID = destinationLocation; ;
        }
        else
        {
            EnemyData enemyData = (EnemyData)targetData;
            BattleManager.Instance.Replace(BattleManager.Instance.CurrentEnemyList, toLocation, destinationLocation);
            enemyData.EnemyTrans.DOAnchorPos(destination, 0.2f);
        }
        EventManager.Instance.DispatchEvent(EventDefinition.eventMove);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    public string SetDescriptionText()
    {
        throw new System.NotImplementedException();
    }
    public string SetTitleText()
    {
        throw new System.NotImplementedException();
    }
}
