using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UIOption : UIBase
{
    [SerializeField]
    private Button exitButton;
    [SerializeField]
    private Button returnButton;
    [SerializeField]
    private Slider bgmSlider;
    [SerializeField]
    private Slider seSlider;
    [SerializeField]
    private Sprite quitSprite;
    [SerializeField]
    private Sprite returnStartMenuSprite;
    private static UIOption instance;
    private bool isStartSceneLoading;
    private void Awake()
    {
        Initialize();
        DontDestroyOnLoad(this);
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = (UIOption)this;
        }
        SceneManager.sceneLoaded += JoinUIDict;
    }
    protected override void Start()
    {
        base.Start();
        EventManager.Instance.AddEventRegister(EventDefinition.eventDrawCard, EventDrawCard);
        EventManager.Instance.AddEventRegister(EventDefinition.eventSceneLoading, EventSceneLoading);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && (BattleManager.Instance.MyBattleType != BattleManager.BattleType.DrawCard))
        {
            Show();
        }
        if (isStartSceneLoading && UI.activeSelf)
        {
            Hide();
        }
    }
    public override void Show()
    {
        base.Show();
        exitButton.onClick.RemoveAllListeners();
        switch (SceneManager.GetActiveScene().name)
        {
            case "StartMenu":
                exitButton.image.sprite = quitSprite;
                exitButton.onClick.AddListener(() => Exit(true));
                break;
            case "Level1":
                exitButton.image.sprite = returnStartMenuSprite;
                exitButton.onClick.AddListener(() => Exit(false));
                break;
        }
    }
    private void Initialize()
    {
        returnButton.onClick.AddListener(Hide);
        bgmSlider.onValueChanged.AddListener((float value) => AudioManager.Instance.ChanageAudioVolume("BGM", value));
        seSlider.onValueChanged.AddListener((float value) => AudioManager.Instance.ChanageAudioVolume("SE", value));
    }
    private void Exit(bool isInStartMenu)
    {
        if (BattleManager.Instance.MyBattleType == BattleManager.BattleType.DrawCard || isStartSceneLoading)
        {
            return;
        }
        if (isInStartMenu)
        {
            Application.Quit();
        }
        else
        {
            StartCoroutine(SceneController.Instance.Transition("StartMenu"));
            SaveLoadManager.Instance.Save();
            EventManager.Instance.DispatchEvent(EventDefinition.eventReloadGame);
        }
        UI.SetActive(false);
    }
    private void JoinUIDict(Scene scene, LoadSceneMode mode)
    {
        isStartSceneLoading = false;
        Dictionary<string, UIBase> uiDict = UIManager.Instance.UIDict;
        string typeName = GetType().Name;
        if (!uiDict.ContainsKey(typeName))
        {
            uiDict.Add(typeName, this);
        }
    }
    private void EventDrawCard(params object[] args)
    {
        Hide();
    }
    private void EventSceneLoading(params object[] args)
    {
        isStartSceneLoading = (bool)args[0];
    }
}
