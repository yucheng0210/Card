using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiceRollerUI : UIBase
{
    [SerializeField] Button _rollButton, sumBetButton, doublesBetButton, showButton, bigButton, smallButton, yesButton, noButton;
    [SerializeField] Text _resultsText, _doublesText, gambleResultsText;
    [SerializeField] DiceRoller2D _diceRoller;
    [SerializeField]
    private Transform gambleTypeTrans, sumBetTrans, doublesBetTrans;
    private string currentDialogID = "GAMBLE_0";

    void OnEnable()
    {
        //_rollButton.onClick.AddListener(RollDice);
        sumBetButton.onClick.AddListener(SumBet);
        doublesBetButton.onClick.AddListener(DoublesBet);
        bigButton.onClick.AddListener(RollDice);
        smallButton.onClick.AddListener(RollDice);
        yesButton.onClick.AddListener(RollDice);
        noButton.onClick.AddListener(RollDice);
        _diceRoller.OnRoll += HandleRoll;
        showButton.onClick.AddListener(Show);
    }
    public override void Show()
    {
        base.Show();
        EventManager.Instance.DispatchEvent(EventDefinition.eventDialog, currentDialogID);
    }
    void OnDisable()
    {
        _rollButton.onClick.RemoveListener(RollDice);
        _diceRoller.OnRoll -= HandleRoll;
    }

    void HandleRoll(int obj)
    {
        _resultsText.text = $"點數為 {obj}";
        _doublesText.text = _diceRoller.Doubles ? "Doubles!" : "";
        gambleResultsText.text = _diceRoller.SumBet + "，" + (_diceRoller.IsWinner ? "你贏了!!!" : "你輸了...");
    }

    void RollDice()
    {
        ClearResults();
        _diceRoller.RollDice();
    }

    void ClearResults()
    {
        bigButton.onClick.RemoveAllListeners();
        smallButton.onClick.RemoveAllListeners();
        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();
        _resultsText.text = "";
        _doublesText.text = "";
    }
    private void SumBet()
    {
        sumBetTrans.gameObject.SetActive(true);
        gambleTypeTrans.gameObject.SetActive(false);
    }
    private void DoublesBet()
    {
        doublesBetTrans.gameObject.SetActive(true);
        gambleTypeTrans.gameObject.SetActive(false);
    }
}
