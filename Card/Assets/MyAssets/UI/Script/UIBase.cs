using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    [SerializeField]
    private GameObject menu;

    protected virtual void Start()
    {
        UIManager.Instance.UIList.Add(this);
    }

    protected virtual void Open()
    {
        menu.gameObject.SetActive(true);
    }

    protected virtual void Close()
    {
        menu.gameObject.SetActive(false);
    }
}
