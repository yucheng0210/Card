using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public abstract class UIBase : MonoBehaviour
{
    [SerializeField]
    private GameObject ui;
    public GameObject UI
    {
        get { return ui; }
    }

    protected virtual void Start()
    {
        string typeName = GetType().Name;
        Dictionary<string, UIBase> uiDict = UIManager.Instance.UIDict;
        if (!uiDict.ContainsKey(typeName))
        {
            uiDict.Add(typeName, this);
        }
    }

    public virtual void Show()
    {
        ui.SetActive(true);
    }

    public virtual void Hide()
    {
        ui.SetActive(false);
    }
}
