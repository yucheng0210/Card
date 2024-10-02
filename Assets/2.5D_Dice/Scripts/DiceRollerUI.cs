using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiceRollerUI : UIBase
{
    [SerializeField]
    Button _rollButton, sumBetButton, doublesBetButton, showButton, tenButton, fiftyButton, hundredButton, allinButton, bigButton, smallButton, yesButton, noButton;
    [SerializeField]
    Text _resultsText, _doublesText, gambleResultsText;
    [SerializeField]
    DiceRoller2D _diceRoller;
    [SerializeField]
    Transform gambleTypeTrans, sumBetTrans, doublesBetTrans, betMoneyTrans;

    private string currentDialogID = "GAMBLE_0";

    void OnEnable()
    {
        RegisterButtonListeners();
        _diceRoller.OnRoll += HandleRoll;
        showButton.onClick.AddListener(Show);
    }

    void OnDisable()
    {
        UnregisterButtonListeners();
        _diceRoller.OnRoll -= HandleRoll;
    }

    private void RegisterButtonListeners()
    {
        tenButton.onClick.AddListener(() => BetMoney(10));
        fiftyButton.onClick.AddListener(() => BetMoney(50));
        hundredButton.onClick.AddListener(() => BetMoney(100));
        allinButton.onClick.AddListener(() => BetMoney(DataManager.Instance.MoneyCount));
        sumBetButton.onClick.AddListener(SumBet);
        doublesBetButton.onClick.AddListener(DoublesBet);

        bigButton.onClick.AddListener(RollDice);
        smallButton.onClick.AddListener(RollDice);
        yesButton.onClick.AddListener(RollDice);
        noButton.onClick.AddListener(RollDice);
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
    }

    public override void Show()
    {
        base.Show();
        EventManager.Instance.DispatchEvent(EventDefinition.eventDialog, currentDialogID);
    }

    private void HandleRoll(int rollResult)
    {
        _resultsText.text = $"點數為 {rollResult}";
        _doublesText.text = _diceRoller.Doubles ? "Doubles!" : "";
        gambleResultsText.text = $"{_diceRoller.SumBet}，{(_diceRoller.IsWinner ? "你贏了!!!" : "你輸了...")}";
    }

    private void RollDice()
    {
        ClearResults();
        _diceRoller.RollDice();
    }

    private void ClearResults()
    {
        bigButton.onClick.RemoveAllListeners();
        smallButton.onClick.RemoveAllListeners();
        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();

        _resultsText.text = "";
        _doublesText.text = "";
    }

    private void BetMoney(int amount)
    {
        if (DataManager.Instance.MoneyCount < amount)
            return;

        gambleTypeTrans.gameObject.SetActive(true);
        betMoneyTrans.gameObject.SetActive(false);

        DataManager.Instance.MoneyCount -= amount;
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    private void SumBet()
    {
        ShowBetType(sumBetTrans);
    }

    private void DoublesBet()
    {
        ShowBetType(doublesBetTrans);
    }

    private void ShowBetType(Transform betType)
    {
        betType.gameObject.SetActive(true);
        gambleTypeTrans.gameObject.SetActive(false);
    }
}
