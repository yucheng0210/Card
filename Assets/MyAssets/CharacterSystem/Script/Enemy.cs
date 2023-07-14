using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private bool isBoss = false;

    [SerializeField]
    private Slider enemyHealthSlider;

    [SerializeField]
    private Text enemyHealthText;

    [SerializeField]
    private Text enemyAttackIntentText;

    [SerializeField]
    private RectTransform enemyHealthRect;

    [SerializeField]
    private RectTransform enemyHurtRect;
    private EnemyData enemy;
    private SkinnedMeshRenderer skinMesh;
    public int EnemyID { get; set; }

    public enum AttackType
    {
        Attack,
        Shield,
        Effect
    }

    private void Awake()
    {
        skinMesh = GetComponentInChildren<SkinnedMeshRenderer>();
        EventManager.Instance.AddEventRegister(EventDefinition.eventTakeDamage, EventTakeDamage);
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
        EventManager.Instance.AddEventRegister(EventDefinition.eventRefreshUI, EventRefreshUI);
    }

    private void Start()
    {
        enemy = DataManager.Instance.EnemyList[EnemyID];
        enemy.CurrentHealth = enemy.MaxHealth;
    }

    private void Update()
    {
        if (enemyHealthRect == null)
            return;
        enemyHurtRect.anchorMax = new Vector2(
            Mathf.Lerp(enemyHurtRect.anchorMax.x, enemyHealthRect.anchorMax.x, Time.deltaTime * 5),
            enemyHurtRect.anchorMax.y
        );
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
            BattleManager.Instance.RemoveEnemy(EnemyID, isBoss);
            Destroy(gameObject, 1);
        }
    }

    private void EventPlayerTurn(params object[] args)
    {
        for (int i = 0; i < BattleManager.Instance.CurrentEnemyList.Count; i++)
        {
            int enemyID = BattleManager.Instance.CurrentEnemyList[i];
            int randomAttack = UnityEngine.Random.Range(
                DataManager.Instance.EnemyList[enemyID].MinAttack,
                DataManager.Instance.EnemyList[enemyID].MaxAttack + 1
            );
            DataManager.Instance.EnemyList[enemyID].CurrentAttack = randomAttack;
            enemyAttackIntentText.text = randomAttack.ToString();
            Debug.Log(randomAttack);
        }
    }

    private void EventRefreshUI(params object[] args)
    {
        enemyHealthSlider.value = (float)(
            (float)DataManager.Instance.EnemyList[EnemyID].CurrentHealth
            / DataManager.Instance.EnemyList[EnemyID].MaxHealth
        );
        enemyHealthText.text =
            DataManager.Instance.EnemyList[EnemyID].CurrentHealth.ToString()
            + "/"
            + DataManager.Instance.EnemyList[EnemyID].MaxHealth.ToString();
    }
}
