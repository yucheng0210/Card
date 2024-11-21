using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResurrectionEffect : IEffect
{
    private int recoverCount;
    private EnemyData enemyData;
    private Enemy enemy;
    private Image enemyEffectImage;
    private string targetLocation;
    private string typeName;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        if (!BattleManager.Instance.CurrentEnemyList.TryGetValue(fromLocation, out enemyData))
        {
            return;
        }
        typeName = GetType().Name;
        targetLocation = fromLocation;
        recoverCount = Mathf.RoundToInt(enemyData.MaxHealth * (value / 100f));
        enemy = enemyData.EnemyTrans.GetComponent<Enemy>();
        enemyEffectImage = enemy.EnemyEffectImage.GetComponent<Image>();
        enemy.EnemyOnceBattlePositiveList.Add(typeName, 1);
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
        EventManager.Instance.AddEventRegister(EventDefinition.eventEnemyTurn, EventEnemyTurn);
    }
    private void EventTakeDamage(params object[] args)
    {
        if (enemyData.CurrentHealth > 0)
        {
            return;
        }
        enemy.MyActionType = Enemy.ActionType.None;
        enemyEffectImage.sprite = ((IEffect)this).SetIcon();
        enemy.EnemyEffectImage.SetActive(true);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
        EventManager.Instance.RemoveEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
    }
    private void EventEnemyTurn(params object[] args)
    {
        if (enemyData.CurrentHealth > 0)
        {
            return;
        }
        enemy.EnemyOnceBattlePositiveList.Remove(typeName);
        // 敌人复活
        enemy.IsDeath = false;
        enemy.MyAnimator.SetTrigger("isResurrection");
        BattleManager.Instance.Recover(enemyData, recoverCount, targetLocation);
        // 触发其他必要的事件
        //EventManager.Instance.DispatchEvent(EventDefinition.eventMove);
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
