using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class KnockBackEffect : IEffect
{
    public void ApplyEffect(int value, string target)
    {
        if (!BattleManager.Instance.CurrentEnemyList.ContainsKey(target))
            return;
        CharacterData targetData;
        int[] playerPos = BattleManager.Instance.ConvertNormalPos(BattleManager.Instance.CurrentLocationID);
        int[] enemyPos = BattleManager.Instance.ConvertNormalPos(target);
        Vector2 direction;
        Vector2Int destinationPoint;
        string locationID;
        if (target == BattleManager.Instance.CurrentLocationID)
        {
            targetData = BattleManager.Instance.CurrentPlayerData;
            direction = new Vector2(playerPos[0] - enemyPos[0], playerPos[1] - enemyPos[1]).normalized;
            destinationPoint = new Vector2Int(playerPos[0] + (int)direction.x, playerPos[1] + (int)direction.y);
        }
        else
        {
            targetData = BattleManager.Instance.CurrentEnemyList[target];
            direction = new Vector2(enemyPos[0] - playerPos[0], enemyPos[1] - playerPos[1]).normalized;
            destinationPoint = new Vector2Int(enemyPos[0] + (int)direction.x, enemyPos[1] + (int)direction.y);
        }
        locationID = BattleManager.Instance.ConvertCheckerboardPos(destinationPoint.x, destinationPoint.y);
        if (!BattleManager.Instance.CheckerboardList.ContainsKey(locationID))
            return;
        int checkerboardPoint = BattleManager.Instance.GetCheckerboardPoint(locationID);
        Vector3 destination = BattleManager.Instance.CheckerboardTrans.GetChild(checkerboardPoint).GetComponent<RectTransform>().localPosition;
        if (target == BattleManager.Instance.CurrentLocationID)
        {
            BattleManager.Instance.PlayerTrans.DOAnchorPos(destination, 0.2f);
            BattleManager.Instance.CurrentLocationID = locationID; ;
        }
        else
        {
            EnemyData enemyData = (EnemyData)targetData;
            BattleManager.Instance.CurrentEnemyList.Add(locationID, enemyData);
            BattleManager.Instance.CurrentEnemyList.Remove(target);
            enemyData.EnemyTrans.DOAnchorPos(destination, 0.2f);
            enemyData.EnemyTrans.GetComponent<Enemy>().EnemyLocation = locationID;
        }
        EventManager.Instance.DispatchEvent(EventDefinition.eventMove);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    public string SetDescriptionText()
    {
        throw new System.NotImplementedException();
    }

    public Sprite SetIcon()
    {
        throw new System.NotImplementedException();
    }

    public string SetTitleText()
    {
        throw new System.NotImplementedException();
    }
}
