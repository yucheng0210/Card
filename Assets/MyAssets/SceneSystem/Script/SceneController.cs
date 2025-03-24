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
    private Image progressCanvasImage;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        progressCanvasImage = progressCanvas.GetComponent<Image>();
    }

    public IEnumerator Transition(string sceneName)
    {
        progressCanvasImage.raycastTarget = true;
        progressSlider.gameObject.SetActive(true);
        progressSlider.value = 0.0f;
        progressText.text = 0 + "%";
        EventManager.Instance.DispatchEvent(EventDefinition.eventSceneLoading, true);
        yield return StartCoroutine(UIManager.Instance.FadeOut(sceneFaderPrefab, 1));
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        async.completed += x => { StartCoroutine(TransitionDone()); };
        async.allowSceneActivation = false;
        while (!async.isDone)
        {
            if (progressSlider.value > 0.99f)
            {
                async.allowSceneActivation = true;
            }
            progressSlider.value = Mathf.Lerp(progressSlider.value, async.progress / 9 * 10, Time.deltaTime);
            progressText.text = (int)(progressSlider.value * 100) + "%";
            yield return null;
        }
    }
    private IEnumerator TransitionDone()
    {
        progressSlider.value = 1.0f;
        progressText.text = (int)(progressSlider.value * 100) + "%";
        yield return StartCoroutine(UIManager.Instance.FadeIn(sceneFaderPrefab, 1, false));
        EventManager.Instance.DispatchEvent(EventDefinition.eventSceneLoading, false);
        progressCanvasImage.raycastTarget = false;
        progressSlider.gameObject.SetActive(false);
    }
}
