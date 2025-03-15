using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField]
    private Transform statusClueTrans;
    [SerializeField]
    private Transform attackStartTrans;
    public Transform StatusClueTrans { get { return statusClueTrans; } set { statusClueTrans = value; } }
    public Transform AttackStartTrans { get { return attackStartTrans; } set { attackStartTrans = value; } }
}
