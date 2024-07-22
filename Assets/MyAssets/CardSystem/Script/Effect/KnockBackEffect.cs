using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class KnockBackEffect : IEffect
{
    public void ApplyEffect(int value, string target)
    {
        if (!BattleManager.Instance.CurrentEnemyList.ContainsKey(target))
            return;
        EnemyData enemyData = BattleManager.Instance.CurrentEnemyList[target];
        int[] playerPos = BattleManager.Instance.ConvertNormalPos(BattleManager.Instance.CurrentLocationID);
        int[] enemyPos = BattleManager.Instance.ConvertNormalPos(target);
        Vector2 direction = new Vector2(enemyPos[0] - playerPos[0], enemyPos[1] - playerPos[1]).normalized;
        Vector2Int destinationPoint = new Vector2Int(enemyPos[0] + (int)direction.x, enemyPos[1] + (int)direction.y);
        string locationID = BattleManager.Instance.ConvertCheckerboardPos(destinationPoint.x, destinationPoint[1]);
        if (!BattleManager.Instance.CheckerboardList.ContainsKey(locationID))
            return;
        int checkerboardPoint = BattleManager.Instance.GetCheckerboardPoint(locationID);
        Vector3 destination = BattleManager.Instance.CheckerboardTrans.GetChild(checkerboardPoint).GetComponent<RectTransform>().localPosition;
        BattleManager.Instance.CurrentEnemyList.Add(locationID, enemyData);
        BattleManager.Instance.CurrentEnemyList.Remove(target);
        enemyData = BattleManager.Instance.CurrentEnemyList[locationID];
        enemyData.EnemyTrans.DOAnchorPos(destination, 0.2f);
        enemyData.EnemyTrans.GetComponent<Enemy>().EnemyLocation = locationID;
        EventManager.Instance.DispatchEvent(EventDefinition.eventMove);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    public Sprite SetIcon()
    {
        throw new System.NotImplementedException();
    }
}
