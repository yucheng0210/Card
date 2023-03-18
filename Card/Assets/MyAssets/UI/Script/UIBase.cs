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
        UIManager.Instance.UIList.Add(this);
    }

    protected virtual void Open()
    {
        ui.gameObject.SetActive(true);
    }

    protected virtual void Close()
    {
        ui.gameObject.SetActive(false);
    }
}
