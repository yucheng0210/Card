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
    }

    public void OnSelect()
    {
        skinMesh.material.SetColor("_OtlColor", Color.red);
    }

    public void OnUnSelect()
    {
        skinMesh.material.SetColor("_OtlColor", Color.black);
    }
}
