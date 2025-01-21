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
        string destinationLocation = GetValidDestination(initialPos, limitedDirection) ?? throw new System.Exception("No valid destination found in CheckerboardList.");
        Vector3 destinationPos = BattleManager.Instance.GetCheckerboardTrans(destinationLocation).localPosition;
        if (toLocation == BattleManager.Instance.CurrentLocationID)
        {
            BattleManager.Instance.PlayerTrans.DOAnchorPos(destinationPos, 0.2f);
            BattleManager.Instance.CurrentLocationID = destinationLocation;
            ShowStatusClue(BattleManager.Instance.CurrentPlayer.StatusClueTrans);
        }
        else
        {
            EnemyData enemyData = (EnemyData)targetData;
            BattleManager.Instance.Replace(BattleManager.Instance.CurrentEnemyList, toLocation, destinationLocation);
            enemyData.EnemyTrans.DOAnchorPos(destinationPos, 0.2f);
            ShowStatusClue(enemyData.EnemyTrans);
        }

        // 派發事件
        EventManager.Instance.DispatchEvent(EventDefinition.eventMove);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    // 獲取有效目標位置
    private string GetValidDestination(Vector2Int initialPos, Vector2Int direction)
    {
        Vector2Int[] offsets = new Vector2Int[]
        {
            new(direction.x, direction.y),  // 原始方向
            new(direction.x, 0),           // 水平
            new(direction.x, -direction.y),// 右下
            new(0, -direction.y),          // 垂直下
            new(-direction.x, -direction.y),// 左下
            new(-direction.x, 0),          // 水平左
            new(-direction.x, direction.y),// 左上
            new(0, direction.y)            // 垂直上
        };
        Vector2Int[] newLocationList = new Vector2Int[offsets.Length];
        for (int i = 0; i < offsets.Length; i++)
        {
            newLocationList[i] = initialPos + offsets[i];
        }
        for (int i = 0; i < newLocationList.Length; i++)
        {
            string newLocation = BattleManager.Instance.ConvertCheckerboardPos(newLocationList[i].x, newLocationList[i].y);
            if (BattleManager.Instance.CheckerboardList.ContainsKey(newLocation) && BattleManager.Instance.CheckPlaceEmpty(newLocation, BattleManager.CheckEmptyType.Move))
            {
                return newLocation;
            }
        }
        return null; // 無效位置
    }

    // 顯示角色狀態提示
    private void ShowStatusClue(Transform characterTrans)
    {
        BattleManager.Instance.ShowCharacterStatusClue(characterTrans, EffectFactory.Instance.CreateEffect(GetType().Name).SetTitleText(), 0);
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
