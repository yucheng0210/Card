using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UIStartMenu : UIBase
{
    [SerializeField]
    private Button startButton;

    [SerializeField]
    private Button optionButton;
    [SerializeField]
    private Button exitButton;
    [SerializeField]
    private Button loadButton;

    protected override void Start()
    {
        base.Start();
        startButton.onClick.AddListener(StartGame);
        exitButton.onClick.AddListener(ExitGame);
        optionButton.onClick.AddListener(() => UIManager.Instance.UIDict["UIOption"].Show());
        loadButton.onClick.AddListener(() => UIManager.Instance.UIDict["UISaveLoad"].Show());
        AudioManager.Instance.BGMAudio(0);
    }
    private void StartGame()
    {
        startButton.onClick.RemoveAllListeners();
        //SceneManager.LoadScene("Level1");
        SaveLoadManager.Instance.CurrentPathID = SaveLoadManager.Instance.GetSaveFileCount();
        SaveLoadManager.Instance.IsLoad = false;
        StartCoroutine(SceneController.Instance.Transition("Level1"));
    }
    private void ExitGame()
    {
        //UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
