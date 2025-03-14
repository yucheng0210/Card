using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class KnockBackEffect : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        CharacterData targetData = BattleManager.Instance.IdentifyCharacter(toLocation);
        if (targetData == null)
        {
            return;
        }
        int[] attackerPos = BattleManager.Instance.ConvertNormalPos(fromLocation);
        int[] defenderPos = BattleManager.Instance.ConvertNormalPos(toLocation);
        Vector2 direction = new(defenderPos[0] - attackerPos[0], defenderPos[1] - attackerPos[1]);
        int limitedX = Mathf.Clamp(Mathf.RoundToInt(direction.x), -1, 1);
        int limitedY = Mathf.Clamp(Mathf.RoundToInt(direction.y), -1, 1);
        Vector2Int initialPos = new(defenderPos[0], defenderPos[1]);
        Vector2Int limitedDirection = new(limitedX, limitedY);
        string defaultDestinationLocation = BattleManager.Instance.ConvertCheckerboardPos((initialPos + limitedDirection).x, (initialPos + limitedDirection).y);
        string destinationLocation = GetValidDestination(toLocation, defaultDestinationLocation);
        if (fromLocation == destinationLocation)
        {
            destinationLocation = toLocation;
        }
        Vector3 destinationPos = BattleManager.Instance.GetCheckerboardTrans(destinationLocation).localPosition;
        if (toLocation == BattleManager.Instance.CurrentPlayerLocation)
        {
            BattleManager.Instance.PlayerTrans.DOAnchorPos(destinationPos, 0.15f);
            BattleManager.Instance.CurrentPlayerLocation = destinationLocation;
            EventManager.Instance.DispatchEvent(EventDefinition.eventMove);
            //ShowStatusClue(BattleManager.Instance.CurrentPlayer.StatusClueTrans);
        }
        else
        {
            EnemyData enemyData = (EnemyData)targetData;
            BattleManager.Instance.Replace(BattleManager.Instance.CurrentEnemyList, toLocation, destinationLocation);
            enemyData.EnemyTrans.DOAnchorPos(destinationPos, 0.15f);
            EventManager.Instance.DispatchEvent(EventDefinition.eventMove, enemyData);
            //enemy.RefreshAttackIntent();
            // ShowStatusClue(enemy.StatusClueTrans);
        }
        EventManager.Instance.DispatchEvent(EventDefinition.eventMove);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }
    private string GetValidDestination(string initialLocation, string defaultLocation)
    {
        if (BattleManager.Instance.CheckPlaceEmpty(defaultLocation, BattleManager.CheckEmptyType.Move))
        {
            return defaultLocation;
        }
        BattleManager.CheckEmptyType checkEmptyType = BattleManager.CheckEmptyType.Move;
        BattleManager.ActionRangeType actionRangeType = BattleManager.ActionRangeType.Surrounding;
        List<string> emptyPlaceList = BattleManager.Instance.GetActionRangeTypeList(initialLocation, 1, checkEmptyType, actionRangeType);
        string destination = emptyPlaceList[0] != null ? emptyPlaceList[0] : initialLocation;
        return destination;
    }
    public string SetTitleText()
    {
        return "擊退";
    }
    public string SetDescriptionText()
    {
        return "擊退敵人。";
    }
}
