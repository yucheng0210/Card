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
    private static UIOption instance;
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
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && BattleManager.Instance.MyBattleType != BattleManager.BattleType.DrawCard)
        {
            Show();
        }
    }
    private void Initialize()
    {
        exitButton.onClick.AddListener(() => Exit());
        returnButton.onClick.AddListener(Hide);
        bgmSlider.onValueChanged.AddListener((float value) => AudioManager.Instance.ChanageAudioVolume("BGM", value));
        seSlider.onValueChanged.AddListener((float value) => AudioManager.Instance.ChanageAudioVolume("SE", value));
    }
    private void Exit()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "StartMenu":
                Application.Quit();
                break;
            case "Level1":
                StartCoroutine(SceneController.Instance.Transition("StartMenu"));
                SaveLoadManager.Instance.Save();
                EventManager.Instance.DispatchEvent(EventDefinition.eventReloadGame);
                break;
        }
        UI.SetActive(false);
    }
    private void JoinUIDict(Scene scene, LoadSceneMode mode)
    {
        Dictionary<string, UIBase> uiDict = UIManager.Instance.UIDict;
        string typeName = GetType().Name;
        if (!uiDict.ContainsKey(typeName))
        {
            uiDict.Add(typeName, this);
        }
    }
}
