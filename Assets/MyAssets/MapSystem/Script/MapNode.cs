using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode : MonoBehaviour
{
    public int value;
    public MapNode left;
    public MapNode right;
    public int level;
    public bool isUsed = false;

    public Level l;
    public void Reset()
    {
        value = 0;
        left = null;
        right = null;
        level = 0;
        isUsed = false;
        l = null;
    }
    private void Awake()
    {
        // Debug.Log(gameObject.GetComponent<RectTransform>().localPosition);
    }
}
