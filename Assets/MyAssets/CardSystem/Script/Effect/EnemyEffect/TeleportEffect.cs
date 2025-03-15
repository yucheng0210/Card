using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class TeleportEffect : IEffect
{
    private string initialLocation;
    public void ApplyEffect(int value, string fromLocation, string toLocation)
    {
        initialLocation = fromLocation;
        if (value == -1)
        {
            EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
        }
        else
        {
            Teleport();
        }
    }
    private void Teleport()
    {
        float minDistance = BattleManager.Instance.CalculateDistance(initialLocation, BattleManager.Instance.CurrentPlayerLocation);
        Enemy enemy = BattleManager.Instance.CurrentEnemyList[initialLocation].EnemyTrans.GetComponent<Enemy>();
        List<string> teleportList = new();
        for (int i = 0; i < BattleManager.Instance.CheckerboardList.Count; i++)
        {
            string key = BattleManager.Instance.CheckerboardList.ElementAt(i).Key;
            float currentDistance = BattleManager.Instance.CalculateDistance(key, BattleManager.Instance.CurrentPlayerLocation);
            if (BattleManager.Instance.CheckPlaceEmpty(key, BattleManager.CheckEmptyType.Move) && currentDistance >= minDistance)
            {
                teleportList.Add(key);
            }
        }
        int randomIndex = Random.Range(0, teleportList.Count);
        BattleManager.Instance.Replace(BattleManager.Instance.CurrentEnemyList, initialLocation, teleportList[randomIndex]);
        int childCount = BattleManager.Instance.GetCheckerboardPoint(teleportList[randomIndex]);
        RectTransform emptyPlace = BattleManager.Instance.CheckerboardTrans.GetChild(childCount).GetComponent<RectTransform>();
        enemy.EnemyImage.material = enemy.DissolveMaterial;
        TweenCallback tweenCallback = () =>
        {
            TweenCallback endTweenCallback = () => { };
            BattleManager.Instance.CurrentEnemyList[teleportList[randomIndex]].EnemyTrans.DOAnchorPos(emptyPlace.localPosition, 0);
            BattleManager.Instance.SetDissolveMaterial(enemy.DissolveMaterial, 0.0f, 1, endTweenCallback);
            AudioManager.Instance.SEAudio(5);
        };
        BattleManager.Instance.SetDissolveMaterial(enemy.DissolveMaterial, 1.0f, 0, tweenCallback);
        AudioManager.Instance.SEAudio(5);
    }
    private void EventTakeDamage(params object[] args)
    {
        if ((int)args[6] > 0 && args[4] == BattleManager.Instance.CurrentPlayerData)
        {
            Teleport();
        }
    }
    public string SetTitleText()
    {
        return "瞬閃";
    }

    public string SetDescriptionText()
    {
        return "隨機朝遠離敵人的方向瞬移。";
    }

}
