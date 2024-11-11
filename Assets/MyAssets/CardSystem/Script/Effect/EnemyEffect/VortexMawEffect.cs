using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class VortexMawEffect : IEffect
{
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        List<string> emptyPlaceList = BattleManager.Instance.GetActionRangeTypeList(fromLocation, value, BattleManager.CheckEmptyType.EnemyAttack, SetEffectAttackType());
        Dictionary<string, EnemyData> currentEnemyList = BattleManager.Instance.CurrentEnemyList;
        string playerLocation = BattleManager.Instance.CurrentLocationID;
        if (emptyPlaceList.Contains(playerLocation))
        {
            RectTransform enemyRect = currentEnemyList[fromLocation].EnemyTrans.GetComponent<RectTransform>();
            Sequence sequence = DOTween.Sequence();
            sequence.Append(BattleManager.Instance.PlayerTrans.DOAnchorPos(enemyRect.localPosition, 0.25f));
            sequence.AppendCallback(() =>
            {
                EffectFactory.Instance.CreateEffect("KnockBackEffect").ApplyEffect(1, fromLocation, toLocation);
                EffectFactory.Instance.CreateEffect("CantMoveEffect").ApplyEffect(1, fromLocation, toLocation);
            });
            sequence.Play();
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
}
