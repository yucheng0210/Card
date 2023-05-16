using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    [SerializeField]
    private GameObject ui;
    public GameObject UI
    {
        get { return ui; }
    }

    protected virtual void Awake()
    {
        UIManager.Instance.UIDict.Add(this.GetType().Name, this);
    }

    public virtual void Show()
    {
        ui.gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        ui.gameObject.SetActive(false);
    }
}
