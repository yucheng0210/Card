using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField]
    private Transform statusClueTrans;
    public Transform StatusClueTrans { get { return statusClueTrans; } set { statusClueTrans = value; } }
}
