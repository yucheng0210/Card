using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIAttackLine : UIBase
{
    [SerializeField]
    private new Camera camera;
    private Vector2 dragPosition;

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
                GetBezierCurve(startPos, midPos, endPos, i / (float)UI.transform.childCount);
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

    private Vector2 GetBezierCurve(Vector2 start, Vector2 mid, Vector2 end, float t)
    {
        return Mathf.Pow((1.0f - t), 2) * start
            + 2.0f * t * (1.0f - t) * mid
            + Mathf.Pow(t, 2) * end;
    }
}
