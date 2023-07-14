using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>
{
    private GameObject player;

    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private Slider progressSlider;

    [SerializeField]
    private Text progressText;

    [SerializeField]
    private GameObject progressCanvas;

    [SerializeField]
    private CanvasGroup sceneFaderPrefab;
    private Dictionary<int, string> sceneNameDic = new Dictionary<int, string>();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    public IEnumerator Transition(string sceneName)
    {
        //EventManager.Instance.DispatchEvent(EventDefinition.eventSceneLoading);
        //GameObject fade = Instantiate(sceneFaderPrefab.gameObject);
        progressSlider.value = 0.0f;
        yield return StartCoroutine(UIManager.Instance.FadeOut(sceneFaderPrefab, 1));
        //progressCanvas.SetActive(true);
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;
        while (progressSlider.value < 0.99f)
        {
            progressSlider.value = Mathf.Lerp(
                progressSlider.value,
                async.progress / 9 * 10,
                Time.deltaTime
            );
            progressText.text = (int)(progressSlider.value * 100) + "%";
            yield return null;
        }
        progressSlider.value = 1.0f;
        progressText.text = (int)(progressSlider.value * 100) + "%";
        yield return new WaitForSeconds(0.5f);
        //progressCanvas.SetActive(false);
        async.allowSceneActivation = true;
        yield return StartCoroutine(UIManager.Instance.FadeIn(sceneFaderPrefab, 1, false));
    }
}
