using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderFollow : MonoBehaviour
{
    [SerializeField]
    private GameObject role;

    [SerializeField]
    private float height;

    [SerializeField]
    private float xOffset;
    private float currentHeight;
    private Canvas myCamera;
    public float Height
    {
        get { return height; }
        set { height = value; }
    }

    private void Awake()
    {
        currentHeight = height;
        transform.position = role.transform.position + new Vector3(xOffset, currentHeight, 0);
        myCamera = GetComponentInParent<Canvas>();
    }

    private void Update()
    {
        currentHeight = Mathf.Lerp(currentHeight, height, Time.deltaTime * 5);
        transform.position = role.transform.position + new Vector3(xOffset, currentHeight, 0);
    }
}
