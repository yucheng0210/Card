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
        recoverCount = BattleManager.Instance.GetPercentage(enemyData.MaxHealth, value);
        enemy = enemyData.EnemyTrans.GetComponent<Enemy>();
        enemyEffectImage = enemy.EnemyEffectImage.GetComponent<Image>();
        enemy.IsSuspendedAnimation = true;
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, enemyData, EventTakeDamage);
        EventManager.Instance.AddEventRegister(EventDefinition.eventEnemyTurn, enemyData, EventEnemyTurn);
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
        enemy.InfoTitle.text = SetTitleText();
        enemy.InfoDescription.text = SetDescriptionText();
        enemy.SetInfoGroupEventTrigger();
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
        EventManager.Instance.RemoveEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
    }
    private void EventEnemyTurn(params object[] args)
    {
        if (enemyData.CurrentHealth > 0)
        {
            return;
        }
        enemy.IsDeath = false;
        enemy.IsSuspendedAnimation = false;
        enemy.MyCollider.enabled = true;
        enemy.MyAnimator.SetTrigger("isResurrection");
        enemyData.MaxPassiveSkillsList[typeName] = -1;
        BattleManager.Instance.Recover(enemyData, recoverCount, targetLocation);
        BattleManager.Instance.ShowCharacterStatusClue(enemy.StatusClueTrans, "復活", 0);
        AudioManager.Instance.SEAudio(2);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
        EventManager.Instance.RemoveEventRegister(EventDefinition.eventEnemyTurn, EventEnemyTurn);
    }
    public bool IsShowEffectCount() { return false; }
    public string SetTitleText()
    {
        return "死者甦醒";
    }

    public string SetDescriptionText()
    {
        return "死亡時復活，使用後移除技能。";
    }
}
