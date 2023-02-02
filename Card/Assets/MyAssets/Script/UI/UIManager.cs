using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    private Transform canvasTrans;
    private List<UIBase> uiList;

    protected override void Awake()
    {
        base.Awake();
        canvasTrans = GameObject.Find("MainCanvas").transform;
        uiList = new List<UIBase>();
    }

    public UIBase ShowUI<T>(string uiName)
        where T : UIBase
    {
        UIBase ui = Find(uiName);
        if (ui == null)
        {
            GameObject obj =
                Instantiate(Resources.Load("MyAssets/UI/" + uiName), canvasTrans) as GameObject;
            obj.name = uiName;
            ui = obj.AddComponent<UIBase>();
            uiList.Add(ui);
        }
        else
            ui.Show();
        return ui;
    }

    public void HideUI(string uiName)
    {
        UIBase ui = Find(uiName);
        if (ui != null)
            ui.Hide();
    }

    public void CloseUI(string uiName)
    {
        UIBase ui = Find(uiName);
        if (ui != null)
        {
            uiList.Remove(ui);
            Destroy(ui.gameObject);
        }
    }

    public void CloseAllUI()
    {
        for (int i = 0; i < uiList.Count - 1; i++)
        {
            Destroy(uiList[i].gameObject);
        }
        uiList.Clear();
    }

    public UIBase Find(string uiName)
    {
        for (int i = 0; i < uiList.Count; i++)
        {
            if (uiList[i].name == uiName)
                return uiList[i];
        }
        return null;
    }
}
