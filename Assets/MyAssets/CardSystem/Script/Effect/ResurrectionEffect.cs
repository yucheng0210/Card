using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResurrectionEffect : IEffect
{
    private int reciprocalCount = 1;
    private int recoverCount;
    private EnemyData enemyData;
    private Enemy enemy;
    private Image enemyEffectImage;
    private string targetLocation;

    public void ApplyEffect(int value, string target)
    {
        if (!BattleManager.Instance.CurrentEnemyList.TryGetValue(target, out enemyData) || enemyData.CurrentHealth > 0)
            return;
        targetLocation = target;
        recoverCount = Mathf.RoundToInt(enemyData.MaxHealth * (value / 100f));
        enemy = enemyData.EnemyTrans.GetComponent<Enemy>();
        enemyEffectImage = enemy.EnemyEffectImage.GetComponent<Image>();

        // 移除被动技能并设置复活效果图标
        enemyData.PassiveSkills.Remove(GetType().Name);
        enemyEffectImage.sprite = ((IEffect)this).SetIcon();
        enemy.EnemyEffectImage.SetActive(true);

        // 注册敌人回合结束事件
        EventManager.Instance.AddEventRegister(EventDefinition.eventEnemyTurn, EventEnemyTurn);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    private void EventEnemyTurn(params object[] args)
    {
        if (--reciprocalCount > 0) return;

        // 敌人复活
        enemy.IsDeath = false;
        BattleManager.Instance.Recover(enemyData, recoverCount, targetLocation);

        // 播放复活动画
        enemy.MyAnimator.SetTrigger("isResurrection");

        // 触发其他必要的事件
        EventManager.Instance.DispatchEvent(EventDefinition.eventMove);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);

        // 移除事件注册
        EventManager.Instance.RemoveEventRegister(EventDefinition.eventEnemyTurn, EventEnemyTurn);
    }

    public string SetTitleText()
    {
        return "死者甦醒";
    }

    public string SetDescriptionText()
    {
        return "當生命歸零時，角色進入復甦狀態，在自己的回合開始時，角色將以特定比例的生命復活，且該被動技能將被移除。";
    }
}
