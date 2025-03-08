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

    protected override void Start()
    {
        base.Start();
        startButton.onClick.AddListener(StartGame);
        exitButton.onClick.AddListener(ExitGame);
        AudioManager.Instance.BGMAudio(0);
    }
    private void StartGame()
    {
        startButton.onClick.RemoveAllListeners();
        //SceneManager.LoadScene("Level1");
        StartCoroutine(SceneController.Instance.Transition("Level1"));
    }
    private void ExitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        //Application.Quit();
    }
}
