using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class VortexMawEffect : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        Dictionary<string, EnemyData> currentEnemyList = BattleManager.Instance.CurrentEnemyList;
        string playerLocation = BattleManager.Instance.CurrentPlayerLocation;
        Enemy enemy = currentEnemyList[fromLocation].EnemyTrans.GetComponent<Enemy>();
        if (enemy.CurrentActionRangeTypeList.Contains(playerLocation))
        {
            RectTransform enemyRect = currentEnemyList[fromLocation].EnemyTrans.GetComponent<RectTransform>();
            Sequence sequence = DOTween.Sequence();
            sequence.Append(BattleManager.Instance.PlayerTrans.DOAnchorPos(enemyRect.localPosition, 0.15f));
            sequence.AppendCallback(() =>
            {
                BattleManager.CheckEmptyType checkEmptyType = BattleManager.CheckEmptyType.ALLCharacter;
                BattleManager.ActionRangeType actionRangeType = BattleManager.ActionRangeType.Default;
                float distance = BattleManager.Instance.CalculateDistance(fromLocation, playerLocation);
                string destination;
                Vector2 destinationPos;
                if (distance < 2)
                {
                    destination = BattleManager.Instance.CurrentPlayerLocation;
                    destinationPos = BattleManager.Instance.GetCheckerboardTrans(destination).localPosition;
                }
                else
                {
                    destination = BattleManager.Instance.GetCloseLocation(fromLocation, playerLocation, 1, checkEmptyType, actionRangeType);
                    destinationPos = BattleManager.Instance.GetCheckerboardTrans(destination).localPosition;
                }
                BattleManager.Instance.PlayerTrans.DOAnchorPos(destinationPos, 0.15f);
                EffectFactory.Instance.CreateEffect(nameof(CantMoveEffect)).ApplyEffect(1, fromLocation, playerLocation);
                BattleManager.Instance.CurrentPlayerLocation = destination;
            });
        }
    }

    public string SetTitleText()
    {
        return "鯨吞";
    }
    public string SetDescriptionText()
    {
        return "拉近周圍敵人。";
    }
    public BattleManager.ActionRangeType SetEffectAttackType() { return BattleManager.ActionRangeType.Surrounding; }
    public int SetEffectRange() { return 2; }
}
