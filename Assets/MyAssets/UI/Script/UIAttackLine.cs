using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAttackLine : UIBase
{
    [SerializeField]
    private float minDistance = 20f; // 設置最小距離
    [SerializeField]
    private RectTransform headHotSpot;
    public RectTransform HeadHotSpot
    {
        get { return headHotSpot; }
    }
    private int endChildCount = 0;
    protected override void Start()
    {
        base.Start();
        EventManager.Instance.AddEventRegister(EventDefinition.eventAttackLine, EventAttackLine);
    }

    public void SetStartPos(Vector2 pos)
    {
        SetPosition(0, pos);
    }

    public void SetEndPos(Vector2 endPos)
    {
        Vector2 startPos = GetChildPosition(0);
        Vector2 midPos = new Vector2(startPos.x, (startPos.y + endPos.y) * 0.5f);
        Vector2 endOffset = new Vector2(20, -15);
        endPos += BattleManager.Instance.DefaultCursorHotSpot + endOffset;
        UpdateChildTransforms(startPos, midPos, endPos);
        SetPosition(UI.transform.childCount - 1, endPos); // 設置終點位置
        SetChildRotation(UI.transform.childCount - 1, GetChildPosition(endChildCount), endPos);
    }

    private void SetPosition(int childIndex, Vector2 position)
    {
        RectTransform rectTransform = UI.transform.GetChild(childIndex).GetComponent<RectTransform>();
        rectTransform.anchoredPosition = position;
    }

    private Vector2 GetChildPosition(int childIndex)
    {
        RectTransform rectTransform = UI.transform.GetChild(childIndex).GetComponent<RectTransform>();
        return rectTransform.anchoredPosition;
    }

    private void UpdateChildTransforms(Vector2 startPos, Vector2 midPos, Vector2 endPos)
    {
        int childCount = UI.transform.childCount;
        float totalDistance = Vector2.Distance(startPos, endPos);

        // 檢查子物件距離是否小於最小距離
        if (totalDistance / childCount < minDistance)
        {
            // 隱藏多餘的子物件
            int visibleChildCount = Mathf.FloorToInt(totalDistance / minDistance);
            for (int i = 0; i < childCount - 1; i++)
            {
                UI.transform.GetChild(i).gameObject.SetActive(i < visibleChildCount);
            }
            childCount = visibleChildCount; // 更新子物件數量
        }
        endChildCount = childCount <= 1 ? 0 : childCount - 2;
        // 重新計算並設置子物件位置
        for (int i = 0; i < childCount; i++)
        {
            Vector2 newPosition = UIManager.Instance.GetBezierCurve(startPos, midPos, endPos, i / (float)childCount);
            SetPosition(i, newPosition);

            // 計算並設置子物件的旋轉角度
            if (i > 0)
            {
                SetChildRotation(i, GetChildPosition(i - 1), newPosition);
            }
        }
    }

    private void SetChildRotation(int childIndex, Vector2 prevPos, Vector2 currentPos)
    {
        Vector2 dir = (currentPos - prevPos).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
        UI.transform.GetChild(childIndex).eulerAngles = new Vector3(0, 0, angle);
    }

    public void EventAttackLine(params object[] args)
    {
        bool isActive = (bool)args[0];
        UI.SetActive(isActive);

        if (isActive)
        {
            SetStartPos((Vector2)args[1]);
            SetEndPos((Vector2)args[2]);
        }
    }
}