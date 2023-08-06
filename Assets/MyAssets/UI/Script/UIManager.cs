using System.ComponentModel;
using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public Dictionary<string, UIBase> UIDict { get; set; }

    protected override void Awake()
    {
        base.Awake();
        UIDict = new Dictionary<string, UIBase>();
    }

    public void ShowUI(string uiName)
    {
        UIDict[uiName].Show();
    }

    public void HideUI(string uiName)
    {
        UIDict[uiName].Hide();
    }

    public void HideAllUI()
    {
        foreach (var i in UIDict)
        {
            HideUI(i.Key);
        }
    }

    public IEnumerator FadeOutIn(
        CanvasGroup canvasGroup,
        float fadeTime,
        float waitTime,
        bool canDestroy
    )
    {
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.unscaledDeltaTime / fadeTime;
            yield return null;
        }
        yield return new WaitForSecondsRealtime(waitTime);
        while (canvasGroup.alpha != 0)
        {
            canvasGroup.alpha -= Time.unscaledDeltaTime / fadeTime;
            yield return null;
        }
        if (canDestroy)
            Destroy(canvasGroup.gameObject);
    }

    public IEnumerator FadeOut(CanvasGroup canvasGroup, float fadeTime)
    {
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.unscaledDeltaTime / fadeTime;
            yield return null;
        }
    }

    public IEnumerator FadeIn(CanvasGroup canvasGroup, float fadeTime)
    {
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.unscaledDeltaTime / fadeTime;
            yield return null;
        }
    }

    public Vector2 GetBezierCurve(Vector2 start, Vector2 mid, Vector2 end, float t)
    {
        return Mathf.Pow(1.0f - t, 2) * start
            + 2.0f * t * (1.0f - t) * mid
            + Mathf.Pow(t, 2) * end;
    }
}
