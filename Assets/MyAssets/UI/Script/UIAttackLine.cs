using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIAttackLine : UIBase
{
    protected override void Start()
    {
        base.Start();
        EventManager.Instance.AddEventRegister(EventDefinition.eventAttackLine, EventAttackLine);
    }

    public void SetStartPos(Vector2 pos)
    {
        UI.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = pos;
    }

    public void SetEndPos(Vector2 pos)
    {
        UI.transform
            .GetChild(UI.transform.childCount - 1)
            .GetComponent<RectTransform>()
            .anchoredPosition = pos;
        Vector2 startPos = UI.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition;
        Vector2 endPos = pos;
        Vector2 midPos = Vector2.zero;
        midPos.x = startPos.x;
        midPos.y = (startPos.y + endPos.y) * 0.5f;
        Vector2 dir = (endPos - startPos).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        UI.transform.GetChild(UI.transform.childCount - 1).eulerAngles = new Vector3(
            0,
            0,
            angle - 90
        );
        for (int i = UI.transform.childCount - 1; i >= 0; i--)
        {
            UI.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition =
                UIManager.Instance.GetBezierCurve(
                    startPos,
                    midPos,
                    endPos,
                    i / (float)UI.transform.childCount
                );
            if (i != UI.transform.childCount - 1)
            {
                dir = (
                    UI.transform.GetChild(i + 1).GetComponent<RectTransform>().anchoredPosition
                    - UI.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition
                ).normalized;
                angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                UI.transform.GetChild(i).eulerAngles = new Vector3(0, 0, angle - 90);
            }
        }
    }

    public void EventAttackLine(params object[] args)
    {
        if ((bool)args[0])
            UI.SetActive(true);
        else
            UI.SetActive(false);
        SetStartPos((Vector2)args[1]);
        SetEndPos((Vector2)args[2]);
    }
}
