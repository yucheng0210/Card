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

    private void Awake() {
        // Debug.Log(gameObject.GetComponent<RectTransform>().localPosition);
    }
}
