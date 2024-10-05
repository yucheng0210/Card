using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiceRollerUI : UIBase
{
    [SerializeField]
    Button _rollButton, sumBetButton, doublesBetButton, showButton, tenButton, fiftyButton, hundredButton, allinButton, bigButton
     , smallButton, yesButton, noButton, exitButton;
    [SerializeField]
    Text _resultsText, _doublesText, gambleResultsText;
    [SerializeField]
    DiceRoller2D _diceRoller;
    [SerializeField]
    Transform gambleTypeTrans, sumBetTrans, doublesBetTrans, betMoneyTrans;
    private int betMoneyCount;
    private float currentOdds;
    private string guest = "";
    // private string currentDialogID = "GAMBLE_0";
    private enum GambleType
    {
        SumBet,
        DoublesBet
    }
    private GambleType currentType;
    void OnEnable()
    {
        _diceRoller.OnRoll += HandleRoll;
        showButton.onClick.AddListener(Show);
        RegisterButtonListeners();
    }

    void OnDisable()
    {
        UnregisterButtonListeners();
        _diceRoller.OnRoll -= HandleRoll;
    }

    private void RegisterButtonListeners()
    {
        betMoneyCount = 0;
        _resultsText.text = "";
        _doublesText.text = "";
        gambleResultsText.text = "";
        currentOdds = 1;
        tenButton.onClick.AddListener(() => BetMoney(10));
        fiftyButton.onClick.AddListener(() => BetMoney(50));
        hundredButton.onClick.AddListener(() => BetMoney(100));
        allinButton.onClick.AddListener(() => BetMoney(DataManager.Instance.MoneyCount));
        sumBetButton.onClick.AddListener(SumBet);
        doublesBetButton.onClick.AddListener(DoublesBet);

        bigButton.onClick.AddListener(() => RollDice("大", 1.5f));
        smallButton.onClick.AddListener(() => RollDice("小", 1.5f));
        yesButton.onClick.AddListener(() => RollDice("True", 5));
        noButton.onClick.AddListener(() => RollDice("False", 5));
    }

    private void UnregisterButtonListeners()
    {
        tenButton.onClick.RemoveAllListeners();
        fiftyButton.onClick.RemoveAllListeners();
        hundredButton.onClick.RemoveAllListeners();
        allinButton.onClick.RemoveAllListeners();
        sumBetButton.onClick.RemoveAllListeners();
        doublesBetButton.onClick.RemoveAllListeners();

        bigButton.onClick.RemoveAllListeners();
        smallButton.onClick.RemoveAllListeners();
        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();
        showButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();
    }

    public override void Show()
    {
        base.Show();
        //EventManager.Instance.DispatchEvent(EventDefinition.eventDialog, currentDialogID);
    }

    private void HandleRoll(int rollResult)
    {
        _resultsText.text = $"點數為 {rollResult}";
        _doublesText.text = _diceRoller.Doubles ? "Doubles!" : "";
        switch (currentType)
        {
            case GambleType.SumBet:
                _diceRoller.IsWinner = _diceRoller.SumBet == guest ? true : false;
                gambleResultsText.text = $"{_diceRoller.SumBet}，{(_diceRoller.IsWinner ? "你贏了!!!" : "你輸了...")}";
                break;
            case GambleType.DoublesBet:
                _diceRoller.IsWinner = _diceRoller.Doubles.ToString() == guest ? true : false;
                gambleResultsText.text = $"{(_diceRoller.Doubles ? "Doubles!" : "Not Doubles!")}，{(_diceRoller.IsWinner ? "你贏了!!!" : "你輸了...")}";
                break;
        }
        if (_diceRoller.IsWinner)
            DataManager.Instance.MoneyCount += (int)(betMoneyCount * currentOdds);
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
        exitButton.gameObject.SetActive(true);
        exitButton.onClick.AddListener(() => BattleManager.Instance.NextLevel("DiceRollerUI"));
        exitButton.onClick.AddListener(() => exitButton.gameObject.SetActive(false));
    }

    private void RollDice(string gamble, float odds)
    {
        UnregisterButtonListeners();
        _diceRoller.RollDice();
        guest = gamble;
        currentOdds = odds;
    }

    private void BetMoney(int amount)
    {
        if (DataManager.Instance.MoneyCount < amount)
            return;

        gambleTypeTrans.gameObject.SetActive(true);
        betMoneyTrans.gameObject.SetActive(false);

        DataManager.Instance.MoneyCount -= amount;
        betMoneyCount = amount;
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    private void SumBet()
    {
        ShowBetType(sumBetTrans, GambleType.SumBet);
    }

    private void DoublesBet()
    {
        ShowBetType(doublesBetTrans, GambleType.DoublesBet);
    }

    private void ShowBetType(Transform betType, GambleType gambleType)
    {
        betType.gameObject.SetActive(true);
        gambleTypeTrans.gameObject.SetActive(false);
        currentType = gambleType;
    }
}
