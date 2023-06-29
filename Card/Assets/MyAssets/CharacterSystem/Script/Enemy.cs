using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private int enemyID;
    private EnemyData enemy;
    private SkinnedMeshRenderer skinMesh;

    public enum AttackType
    {
        Attack,
        Shield,
        Effect
    }

    private void Start()
    {
        enemy = DataManager.Instance.EnemyList[enemyID];
        enemy.CurrentHealth = enemy.MaxHealth;
        skinMesh = GetComponentInChildren<SkinnedMeshRenderer>();
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
    }

    public void OnSelect()
    {
        skinMesh.material.SetColor("_OtlColor", Color.red);
    }

    public void OnUnSelect()
    {
        skinMesh.material.SetColor("_OtlColor", Color.black);
    }

    private void EventTakeDamage(params object[] args)
    {
        if (enemy.CurrentHealth <= 0)
        {
            BattleManager.Instance.RemoveEnemy(enemyID);
            Destroy(gameObject, 1);
        }
    }
}
