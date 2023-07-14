using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialog
{
    [SerializeField]
    private string branch;

    [SerializeField]
    private string type;

    [SerializeField]
    private string theName;

    [SerializeField]
    private string content;

    [SerializeField]
    private string order;

    #region Read from Dialog

    public string Branch
    {
        get { return branch; }
        set { branch = value; }
    }
    public string Type
    {
        get { return type; }
        set { type = value; }
    }
    public string TheName
    {
        get { return theName; }
        set { theName = value; }
    }
    public string Content
    {
        get { return content; }
        set { content = value; }
    }
    public string Order
    {
        get { return order; }
        set { order = value; }
    }
    #endregion
}
