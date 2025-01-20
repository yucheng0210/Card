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

        // 計算擊退方向
        Vector2 direction = new(defenderPos[0] - attackerPos[0], defenderPos[1] - attackerPos[1]);
        int limitedX = Mathf.Clamp(Mathf.RoundToInt(direction.x), -1, 1);
        int limitedY = Mathf.Clamp(Mathf.RoundToInt(direction.y), -1, 1);
        // 獲取有效目標位置
        Vector2Int initialDestinationPos = new(defenderPos[0] + limitedX, defenderPos[1] + limitedY);
        string destinationLocation = GetValidDestination(initialDestinationPos) ?? throw new System.Exception("No valid destination found in CheckerboardList.");
        // 獲取目標 UI 座標
        int checkerboardPoint = BattleManager.Instance.GetCheckerboardPoint(destinationLocation);
        Vector3 destinationPos = BattleManager.Instance.CheckerboardTrans.GetChild(checkerboardPoint).GetComponent<RectTransform>().localPosition;

        // 處理角色位置與顯示提示
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
    private string GetValidDestination(Vector2Int destinationPoint)
    {
        Vector2Int[] offsets = new Vector2Int[]
        {
            new(0, destinationPoint[1]), new(destinationPoint[0], 0), new(-destinationPoint[0], destinationPoint[1]),
            new(destinationPoint[0], -destinationPoint[1]), new(0, -destinationPoint[1]), new(-destinationPoint[0], 0),
            new(-destinationPoint[0], -destinationPoint[1])
        };

        for (int i = 0; i < offsets.Length; i++)
        {
            string newLocation = BattleManager.Instance.ConvertCheckerboardPos(offsets[i].x, offsets[i].y);
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
