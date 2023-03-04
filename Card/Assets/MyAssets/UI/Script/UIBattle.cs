using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBattle : UIBase
{
    [SerializeField]
    private Text actionPointText;
    public Text ActionPointText
    {
        get { return actionPointText; }
        set { actionPointText = value; }
    }

    protected override void Start()
    {
        base.Start();
        BattleManager.Instance.ConsumeActionPoint(0);
    }
}
