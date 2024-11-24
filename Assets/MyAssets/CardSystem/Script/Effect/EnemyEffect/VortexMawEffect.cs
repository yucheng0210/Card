using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class VortexMawEffect : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        Dictionary<string, EnemyData> currentEnemyList = BattleManager.Instance.CurrentEnemyList;
        string playerLocation = BattleManager.Instance.CurrentLocationID;
        Enemy enemy = currentEnemyList[fromLocation].EnemyTrans.GetComponent<Enemy>();
        if (enemy.CurrentActionRangeTypeList.Contains(playerLocation))
        {
            RectTransform enemyRect = currentEnemyList[fromLocation].EnemyTrans.GetComponent<RectTransform>();
            Sequence sequence = DOTween.Sequence();
            sequence.Append(BattleManager.Instance.PlayerTrans.DOAnchorPos(enemyRect.localPosition, 0.25f));
            sequence.AppendCallback(() =>
            {
                BattleManager.CheckEmptyType checkEmptyType = BattleManager.CheckEmptyType.Move;
                BattleManager.ActionRangeType actionRangeType = BattleManager.ActionRangeType.Surrounding;
                List<string> emptyPlaceList = BattleManager.Instance.GetActionRangeTypeList(fromLocation, 1, checkEmptyType, actionRangeType);
                Vector2 destinationPos = BattleManager.Instance.GetCheckerboardTrans(emptyPlaceList[0]).localPosition;
                BattleManager.Instance.PlayerTrans.DOAnchorPos(destinationPos, 0.25f);
                EffectFactory.Instance.CreateEffect("CantMoveEffect").ApplyEffect(1, fromLocation, toLocation);
                BattleManager.Instance.CurrentLocationID = emptyPlaceList[0];
            });
        }
    }

    public string SetTitleText()
    {
        return "漩渦巨口";
    }
    public string SetDescriptionText()
    {
        return "將周圍的敵人吸引過來。";
    }
    public BattleManager.ActionRangeType SetEffectAttackType() { return BattleManager.ActionRangeType.Surrounding; }
    public int SetEffectRange() { return 2; }
}
