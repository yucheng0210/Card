using UnityEngine;
using UnityEngine.UI;

public class BattleState : MonoBehaviour
{
    [SerializeField]
    private Image battleStateImage;
    [SerializeField]
    private Text battleStateAmount;
    [SerializeField]
    private Transform infoGroupTrans;
    [SerializeField]
    private Text infoTitle;
    [SerializeField]
    private Text infoDescription;
    [SerializeField]
    private GameObject disableImage;
    public Image BattleStateImage
    {
        get { return battleStateImage; }
        set { battleStateImage = value; }
    }

    public Text BattleStateAmount
    {
        get { return battleStateAmount; }
        set { battleStateAmount = value; }
    }
    public Transform InfoGroupTrans
    {
        get { return infoGroupTrans; }
        set { infoGroupTrans = value; }
    }
    public Text InfoTitle
    {
        get { return infoTitle; }
        set { infoTitle = value; }
    }

    public Text InfoDescription
    {
        get { return infoDescription; }
        set { infoDescription = value; }
    }
    public GameObject DisableImage
    {
        get { return disableImage; }
        set { disableImage = value; }
    }
}
