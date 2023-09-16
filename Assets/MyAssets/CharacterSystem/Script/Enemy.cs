using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    /*  [SerializeField]
      private bool isBoss = false;*/
    [SerializeField]
    private Image alertImage;
    [SerializeField]
    private Slider enemyHealthSlider;

    [SerializeField]
    private Text enemyHealthText;

    [SerializeField]
    private Text enemyAttackIntentText;

    /* [SerializeField]
     private RectTransform enemyHealthRect;

     [SerializeField]
     private RectTransform enemyHurtRect;
     private SkinnedMeshRenderer skinMesh;*/
    public Image EnemyAlert
    {
        get { return alertImage; }
        set { alertImage = value; }
    }
    public Image EnemyImage { get; set; }
    public int EnemyID { get; set; }
    public string EnemyLocation { get; set; }

    public enum AttackType
    {
        Attack,
        Shield,
        Effect
    }

    private void Awake()
    {
        //skinMesh = GetComponentInChildren<SkinnedMeshRenderer>();
        EnemyImage = GetComponent<Image>();
        //EventManager.Instance.AddEventRegister(EventDefinition.eventRefreshUI, EventRefreshUI);
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
    }

    /* private void Update()
     {
         if (enemyHealthRect == null)
             return;
         enemyHurtRect.anchorMax = new Vector2(
             Mathf.Lerp(enemyHurtRect.anchorMax.x, enemyHealthRect.anchorMax.x, Time.deltaTime * 5),
             enemyHurtRect.anchorMax.y
         );
     }*/

    private void OnDisable()
    {
        // EventManager.Instance.RemoveEventRegister(EventDefinition.eventRefreshUI, EventRefreshUI);
        EventManager.Instance.RemoveEventRegister(EventDefinition.eventPlayerTurn, EventPlayerTurn);
    }

    public void OnSelect()
    {
        //skinMesh.material.SetColor("_OtlColor", Color.red);
    }

    public void OnUnSelect()
    {
        //skinMesh.material.SetColor("_OtlColor", Color.black);
    }

    private void EventPlayerTurn(params object[] args)
    {
        int randomAttack = UnityEngine.Random.Range(
            DataManager.Instance.EnemyList[EnemyID].MinAttack,
            DataManager.Instance.EnemyList[EnemyID].MaxAttack + 1
        );
        BattleManager.Instance.CurrentEnemyList[EnemyLocation].CurrentAttack = randomAttack;
        enemyAttackIntentText.text = randomAttack.ToString();
    }

    /* private void EventRefreshUI(params object[] args)
     {
         try
         {
             enemyHealthSlider.value = (float)(
                 (float)BattleManager.Instance.CurrentEnemyList[EnemyLocation].CurrentHealth
                 / DataManager.Instance.EnemyList[EnemyID].MaxHealth
             );
             enemyHealthText.text =
                 BattleManager.Instance.CurrentEnemyList[EnemyLocation].CurrentHealth.ToString()
                 + "/"
                 + DataManager.Instance.EnemyList[EnemyID].MaxHealth.ToString();
         }
         catch (ArgumentOutOfRangeException)
         {
             Debug.Log(EnemyLocation + "：" + "錯誤");
         }
     }*/
}
