using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DG.Tweening;
using Cinemachine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering.Universal;
public class Enemy : Character
{

    [SerializeField]
    private GameObject shockWavePrefab;
    [SerializeField]
    private Text enemyAttackIntentText;
    [SerializeField]
    private Image enemyImage;
    [SerializeField]
    private Animator myAnimator;
    [Header("攻擊意圖")]
    [SerializeField]
    private GameObject enemyEffect;
    [SerializeField]
    private GameObject enemyAttack;
    [SerializeField]
    private GameObject enemyShield;
    [SerializeField]
    private GameObject enemyMove;
    [SerializeField]
    private Transform infoGroupTrans;
    [SerializeField]
    private Text infoTitle;
    [SerializeField]
    private Text infoDescription;
    [SerializeField]
    private CinemachineImpulseSource jumpImpulseSource;
    [SerializeField]
    private CinemachineImpulseSource growlImpulseSource;

    public Text InfoTitle { get { return infoTitle; } set { infoTitle = value; } }
    public Text InfoDescription { get { return infoDescription; } set { infoDescription = value; } }
    public Text EnemyAttackIntentText { get { return enemyAttackIntentText; } set { enemyAttackIntentText = value; } }
    public Image EnemyImage { get { return enemyImage; } set { enemyImage = value; } }
    public GameObject EnemyEffectImage { get { return enemyEffect; } set { enemyEffect = value; } }
    public GameObject EnemyAttackImage { get { return enemyAttack; } set { enemyAttack = value; } }
    public GameObject EnemyShieldImage { get { return enemyShield; } set { enemyShield = value; } }
    public Animator MyAnimator { get { return myAnimator; } set { myAnimator = value; } }
    public BoxCollider MyCollider { get; set; }
    public List<string> CurrentActionRangeTypeList { get; set; }
    public int EnemyID { get; set; }
    public ActionType MyActionType { get; set; }
    public BattleManager.ActionRangeType MyNextAttackActionRangeType { get; set; }
    public BattleManager.CheckEmptyType MyCheckEmptyType { get; set; }
    public bool IsDeath { get; set; }
    public Dictionary<string, int> EnemyOnceBattlePositiveList { get; set; }
    public Sequence MySequence { get; set; }
    private Dictionary<string, EnemyData> currentEnemyList = new();
    public EnemyData MyEnemyData { get; set; }
    public EnemyData MasterEnemyData { get; set; }
    public int AdditionPower { get; set; }
    public int AdditionAttackCount { get; set; }
    public int AdditionAttackMultiplier { get; set; }
    public int CurrentActionRange { get; set; }
    public int CurrentAttackPower { get; set; }
    public int CurrentAttackCount { get; set; }
    public int CurrentShieldCount { get; set; }
    public string TemporaryEffect { get; set; }
    public string TargetLocation { get; set; }
    public bool NoNeedCheckInRange { get; set; }
    public bool InRange { get; set; }
    private string location;
    public bool IsSpecialAction { get; set; }
    //public int SpecialActionStage { get; set; }
    public bool IsDizziness { get; set; }
    public bool IsSuspendedAnimation { get; set; }
    public enum ActionType
    {
        Move,
        Attack,
        Shield,
        Effect,
        None,
    }
    private void Start()
    {
        EnemyOnceBattlePositiveList = new Dictionary<string, int>();
        currentEnemyList = BattleManager.Instance.CurrentEnemyList;
        MyEnemyData.CurrentAttackOrderStrs = MyEnemyData.AttackOrderStrs;
        MyCollider = GetComponent<BoxCollider>();
        EventManager.Instance.AddEventRegister(EventDefinition.eventPlayerTurn, MyEnemyData, EventPlayerTurn);
        EventManager.Instance.AddEventRegister(EventDefinition.eventMove, MyEnemyData, EventMove);
        EventManager.Instance.AddEventRegister(EventDefinition.eventRefreshUI, MyEnemyData, EventRefreshUI);
        EventManager.Instance.AddEventRegister(EventDefinition.eventEnemyTurn, MyEnemyData, EventEnemyTurn);
        RefreshAttackIntent();
    }
    private void Update()
    {
        //Debug.Log(MyEnemyData.MaxHealth);
    }
    public void RefreshAttackIntent()
    {
        MySequence = null;
        location = BattleManager.Instance.GetEnemyKey(MyEnemyData);
        float distance = BattleManager.Instance.GetRoute(location, BattleManager.Instance.CurrentPlayerLocation, BattleManager.CheckEmptyType.EnemyAttack).Count;
        HandleAttack(true);
        ResetUIElements();
        BattleManager.Instance.CheckPlayerLocationInRange(this);
        if (distance == 0 || IsDizziness)
        {
            HandleNoAttack();
        }
        else if (InRange || NoNeedCheckInRange)
        {
            HandleAttack(false);
        }
        else
        {
            HandleMove();
        }
        BattleManager.Instance.CheckPlayerLocationInRange(this);
        SetInfoGroupEventTrigger();
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }
    public void ResetUIElements()
    {
        enemyAttack.SetActive(false);
        enemyShield.SetActive(false);
        enemyMove.SetActive(false);
        enemyEffect.SetActive(false);
        enemyAttackIntentText.enabled = true;
    }

    private void HandleNoAttack()
    {
        MyActionType = ActionType.None;
        enemyAttackIntentText.text = "?";
    }

    private void HandleAttack(bool isCheck)
    {
        TargetLocation = BattleManager.Instance.CurrentPlayerLocation;
        string attackOrder;
        if (IsSpecialAction)
        {
            ValueTuple<string, int> triggerSkill = MyEnemyData.SpecialTriggerSkill;
            Dictionary<string, int> mechanismSkill = MyEnemyData.SpecialMechanismList;
            if (triggerSkill.Item1 == "Shield")
            {
                BattleManager.Instance.GetShield(MyEnemyData, triggerSkill.Item2);
            }
            else
            {
                MyActionType = ActionType.Effect;
                BattleManager.CheckEmptyType checkEmptyType = BattleManager.CheckEmptyType.Move;
                BattleManager.ActionRangeType actionRangeType = EffectFactory.Instance.CreateEffect(triggerSkill.Item1).SetEffectAttackType();
                List<string> actionRangeList = BattleManager.Instance.GetActionRangeTypeList(location, 0, checkEmptyType, actionRangeType);
                CurrentActionRangeTypeList = actionRangeList;
                EffectFactory.Instance.CreateEffect(triggerSkill.Item1).ApplyEffect(triggerSkill.Item2, location, TargetLocation);
            }
            for (int i = 0; i < mechanismSkill.Count; i++)
            {
                string key = mechanismSkill.ElementAt(i).Key;
                string clueStrs = EffectFactory.Instance.CreateEffect(key).SetTitleText();
                float waitTime = 0.5f * i;
                EffectFactory.Instance.CreateEffect(key).ApplyEffect(mechanismSkill[key], location, TargetLocation);
                MyEnemyData.MaxPassiveSkillsList.Add(key, mechanismSkill[key]);
                BattleManager.Instance.ShowCharacterStatusClue(StatusClueTrans, clueStrs, waitTime);
            }
            MyEnemyData.CurrentAttackOrderIndex = 0;
            MyEnemyData.CurrentAttackOrderStrs = MyEnemyData.SpecialAttackOrderStrs;
            IsSpecialAction = false;
        }
        attackOrder = MyEnemyData.CurrentAttackOrderStrs.ElementAt(MyEnemyData.CurrentAttackOrderIndex).Item1;
        MyCheckEmptyType = BattleManager.CheckEmptyType.EnemyAttack;
        CurrentActionRange = MyEnemyData.AttackRange;
        if (Enum.TryParse(attackOrder, out BattleManager.ActionRangeType attackType))
        {
            MyNextAttackActionRangeType = attackType;
            MyActionType = ActionType.Attack;
            if (!isCheck)
            {
                ActiveAttack();
            }
        }
        else if (attackOrder == "Shield")
        {
            ActivateShield();
        }
        else
        {
            ActivateEffect(attackOrder);
        }
        SetAttackActionRangeType();
        CurrentActionRangeTypeList = BattleManager.Instance.GetActionRangeTypeList(location, CurrentActionRange, MyCheckEmptyType, MyNextAttackActionRangeType);
    }
    private void ActivateShield()
    {
        int shieldCount = MyEnemyData.CurrentAttackOrderStrs.ElementAt(MyEnemyData.CurrentAttackOrderIndex).Item2;
        infoTitle.text = "護盾";
        infoDescription.text = "產生護盾。";
        MyActionType = ActionType.Shield;
        MyNextAttackActionRangeType = BattleManager.ActionRangeType.None;
        enemyAttackIntentText.enabled = false;
        //enemyAttackIntentText.text = shieldCount.ToString();
        enemyShield.SetActive(true);
        CurrentShieldCount = shieldCount;
    }
    private void ActivateEffect(string attackOrder)
    {
        IEffect effect = EffectFactory.Instance.CreateEffect(attackOrder);
        MyActionType = ActionType.Effect;
        MyNextAttackActionRangeType = effect.SetEffectAttackType();
        CurrentActionRange = effect.SetEffectRange();
        if (CurrentActionRange <= 0)
        {
            CurrentActionRange = MyEnemyData.AttackRange;
        }
        Image enemyEffectImage = enemyEffect.GetComponent<Image>();
        Sprite effectSprite = effect.SetIcon();
        if (effectSprite == null)
        {
            effectSprite = EffectFactory.Instance.CreateEffect("KnockBackEffect").SetIcon();
        }
        infoTitle.text = effect.SetTitleText();
        infoDescription.text = effect.SetDescriptionText(); ;
        enemyAttackIntentText.enabled = false;
        enemyEffectImage.sprite = effectSprite;
        enemyEffect.SetActive(true);
        TargetLocation = BattleManager.Instance.CurrentPlayerLocation;
    }

    private void HandleMove()
    {
        BattleManager.ActionRangeType actionRangeType = BattleManager.ActionRangeType.Default;
        CurrentActionRange = MyEnemyData.StepCount;
        infoTitle.text = "移動";
        infoDescription.text = "進行移動。";
        MyActionType = ActionType.Move;
        MyCheckEmptyType = BattleManager.CheckEmptyType.Move;
        CurrentActionRangeTypeList = BattleManager.Instance.GetActionRangeTypeList(location, CurrentActionRange, MyCheckEmptyType, actionRangeType);
        enemyAttackIntentText.enabled = false;
        enemyMove.SetActive(true);
    }

    public void SetInfoGroupEventTrigger()
    {
        EventTrigger eventTrigger = infoGroupTrans.GetComponent<EventTrigger>();
        UnityAction unityAction_1 = () => { infoGroupTrans.GetChild(0).gameObject.SetActive(true); };
        UnityAction unityAction_2 = () => { infoGroupTrans.GetChild(0).gameObject.SetActive(false); };
        BattleManager.Instance.SetEventTrigger(eventTrigger, unityAction_1, unityAction_2);
    }

    private void ActiveAttack()
    {
        infoTitle.text = "攻擊";
        infoDescription.text = "發動攻擊。";
        CurrentAttackCount = MyEnemyData.CurrentAttackOrderStrs[MyEnemyData.CurrentAttackOrderIndex].Item2;
        BattleManager.Instance.SetEnemyAttackIntentText(this);
        enemyAttack.SetActive(true);
    }
    private void SetAttackActionRangeType()
    {
        MySequence = null;
        switch (MyNextAttackActionRangeType)
        {
            case BattleManager.ActionRangeType.Jump:
                JumpAttackSequence();
                break;
            case BattleManager.ActionRangeType.StraightCharge:
                StraightChargeAttackSequence();
                break;
            case BattleManager.ActionRangeType.ThrowScattering:
                ThrowScatteringAttack();
                break;
        }
    }
    private void JumpAttackSequence()
    {
        MySequence = DOTween.Sequence().Pause();
        string destinationLocation = BattleManager.Instance.CurrentPlayerLocation;
        string enemyLocation = BattleManager.Instance.GetEnemyKey(MyEnemyData);
        float distance = BattleManager.Instance.CalculateDistance(enemyLocation, destinationLocation);
        RectTransform destinationPlace = BattleManager.Instance.PlayerTrans;
        RectTransform enemyRect = GetComponent<RectTransform>();
        int curveHeight = 350;
        Vector2 startPoint = enemyRect.localPosition;
        Vector2 endPoint = destinationPlace.localPosition;
        Vector2 midPoint = new(startPoint.x + distance / 2, startPoint.y + curveHeight);
        Tween moveTween = DOTween.To((t) =>
        {
            Vector2 position = UIManager.Instance.GetBezierCurve(startPoint, midPoint, endPoint, t);
            enemyRect.anchoredPosition = position;
            if (t >= 0.6)
            {
                jumpImpulseSource.GenerateImpulse();
            }
        }, 0, 1, 1).SetEase(Ease.InQuad);
        MySequence.Append(moveTween).AppendCallback(() =>
        {
            OnAttackComplete(true, enemyLocation, destinationLocation);
        }
    );
    }
    private void StraightChargeAttackSequence()
    {
        MySequence = DOTween.Sequence().Pause();
        string enemyLocation = BattleManager.Instance.GetEnemyKey(MyEnemyData);
        int attackDistance = MyEnemyData.AttackRange;
        BattleManager.ActionRangeType actionRangeType = BattleManager.ActionRangeType.Linear;
        List<string> emptyPlaceList = BattleManager.Instance.GetActionRangeTypeList(enemyLocation, attackDistance, MyCheckEmptyType, actionRangeType);
        string playerLocation = BattleManager.Instance.CurrentPlayerLocation;
        string destinationLocation;
        if (emptyPlaceList.Count == 0)
        {
            destinationLocation = enemyLocation;
        }
        else
        {
            destinationLocation = emptyPlaceList[^1];
        }
        RectTransform enemyRect = GetComponent<RectTransform>();
        Vector2 destinationPos = BattleManager.Instance.GetCheckerboardTrans(destinationLocation).localPosition;
        Tween moveTween = enemyRect.DOAnchorPos(destinationPos, 0.1f);
        MySequence.Append(moveTween).AppendCallback(() =>
        {
            Time.timeScale = 0.1f;
            StartCoroutine(ResetTimeScale());
            OnAttackComplete(true, enemyLocation, destinationLocation);
        });
    }
    private IEnumerator ResetTimeScale()
    {
        yield return new WaitForSecondsRealtime(0.15f);
        Time.timeScale = 1f;
    }
    private void ThrowScatteringAttack()
    {
        NoNeedCheckInRange = true;
    }
    private void OnAttackComplete(bool isKnockBack, string startLocation, string endLocation)
    {
        if (InRange)
        {
            string particlePath = MyEnemyData.AttackParticleEffectPath;
            Vector3 destinationPos = new Vector3(transform.position.x, transform.position.y, -1);
            StartCoroutine(BattleManager.Instance.SetParticleEffect(MyAnimator, destinationPos, destinationPos, particlePath, false));
            PlayerData playerData = BattleManager.Instance.CurrentPlayerData;
            if (isKnockBack)
            {
                string attackLocation = endLocation == BattleManager.Instance.CurrentPlayerLocation ? startLocation : endLocation;
                EffectFactory.Instance.CreateEffect(nameof(KnockBackEffect)).ApplyEffect(0, attackLocation, BattleManager.Instance.CurrentPlayerLocation);
            }
            BattleManager.Instance.TakeDamage(MyEnemyData, playerData, CurrentAttackPower, BattleManager.Instance.CurrentPlayerLocation, 0);
            //BattleManager.Instance.CameraImpulse(GetComponent<CinemachineImpulseSource>());
        }
        BattleManager.Instance.Replace(currentEnemyList, startLocation, endLocation);
    }
    public void Growl()
    {
        StartCoroutine(AfterGrowl());
    }
    private IEnumerator AfterGrowl()
    {
        yield return new WaitForSeconds(0.5f);
        jumpImpulseSource.GenerateImpulse();
        enemyImage.enabled = true;
        yield return new WaitForSeconds(1.5f);
        if (BattleManager.Instance.GlobalVolume.profile.TryGet(out ChromaticAberration ca))
        {
            ca.intensity.Override(1);
        }
        AudioManager.Instance.SEAudio(9);
        Material speedLineMaterial = BattleManager.Instance.SpeedLineMaterial;
        speedLineMaterial.color = new Color(speedLineMaterial.color.r, speedLineMaterial.color.g, speedLineMaterial.color.b, 1);
        growlImpulseSource.GenerateImpulse();
        Instantiate(shockWavePrefab, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(3);
        ca.intensity.Override(0);
        speedLineMaterial.color = new Color(speedLineMaterial.color.r, speedLineMaterial.color.g, speedLineMaterial.color.b, 0);
    }
    private void EventPlayerTurn(params object[] args)
    {
        NoNeedCheckInRange = false;
        BattleManager.Instance.RefreshCheckerboardList();
        TemporaryEffect = "";
        RefreshAttackIntent();
    }
    private void EventEnemyTurn(params object[] args)
    {
        if (MyEnemyData.SpecialAttackCondition == -1)
        {
            return;
        }
        if (MyEnemyData.CurrentHealth <= BattleManager.Instance.GetPercentage(MyEnemyData.MaxHealth, MyEnemyData.SpecialAttackCondition))
        {
            //Debug.Log(MyEnemyData.CurrentHealth + "   " + BattleManager.Instance.GetPercentage(MyEnemyData.MaxHealth, MyEnemyData.SpecialAttackCondition));
            IsSpecialAction = true;
            //SpecialActionStage++;
            EventManager.Instance.RemoveEventRegister(EventDefinition.eventEnemyTurn, EventEnemyTurn);
        }
    }
    private void EventMove(params object[] args)
    {
        BattleManager.Instance.CheckPlayerLocationInRange(this);
        BattleManager.Instance.CheckPlayerLocationInTrapRange();
        UIManager.Instance.ClearMoveClue(true);
        BattleManager.Instance.RefreshCheckerboardList();

        if (MyActionType == ActionType.Move)
        {
            HandleMove();
        }
        if (args.Length > 0 && (EnemyData)args[0] == MyEnemyData)
        {
            RefreshAttackIntent();
        }
        //EventManager.Instance.DispatchEvent(EventDefinition.eventAfterMove);
        //SetAttackActionRangeType();
    }
    private void EventRefreshUI(params object[] args)
    {
        if (MyActionType == ActionType.Attack)
        {
            BattleManager.Instance.SetEnemyAttackIntentText(this);
        }
    }
}