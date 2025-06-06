using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiceRollerUI : UIBase
{
    [SerializeField]
    private Text title;
    [SerializeField]
    Button _rollButton, sumBetButton, doublesBetButton, showButton, tenButton, fiftyButton, hundredButton, allinButton, bigButton
     , smallButton, yesButton, noButton, exitButton;
    [SerializeField]
    Text gambleResultsText;
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
    private void Awake()
    {
        showButton.onClick.AddListener(Show);
        _diceRoller.OnRoll += HandleRoll;
    }
    void OnDisable()
    {
        UnregisterButtonListeners();
    }
    public override void Show()
    {
        base.Show();

        RegisterButtonListeners();
        //EventManager.Instance.DispatchEvent(EventDefinition.eventDialog, currentDialogID);
    }
    private void RegisterButtonListeners()
    {
        title.text = "請選擇賭金";
        betMoneyCount = 0;
        gambleResultsText.text = "";
        currentOdds = 1;
        betMoneyTrans.gameObject.SetActive(true);
        gambleTypeTrans.gameObject.SetActive(false);
        sumBetTrans.gameObject.SetActive(false);
        doublesBetTrans.gameObject.SetActive(false);
        tenButton.onClick.AddListener(() => BetMoney(10));
        fiftyButton.onClick.AddListener(() => BetMoney(50));
        hundredButton.onClick.AddListener(() => BetMoney(100));
        allinButton.onClick.AddListener(() => BetMoney(DataManager.Instance.MoneyCount));
        sumBetButton.onClick.AddListener(SumBet);
        doublesBetButton.onClick.AddListener(DoublesBet);

        bigButton.onClick.AddListener(() => RollDice("大", 1.5f));
        smallButton.onClick.AddListener(() => RollDice("小", 1.5f));
        yesButton.onClick.AddListener(() => RollDice("True", 5));
        noButton.onClick.AddListener(() => RollDice("False", 1.1f));
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
        //showButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();
    }

    private void HandleRoll(int rollResult)
    {
        title.text = $"點數為 {rollResult}";
        switch (currentType)
        {
            case GambleType.SumBet:
                _diceRoller.IsWinner = _diceRoller.SumBet == guest ? true : false;
                gambleResultsText.text = $"{_diceRoller.SumBet}，{(_diceRoller.IsWinner ? "你贏了!!!" : "你輸了...")}";
                break;
            case GambleType.DoublesBet:
                _diceRoller.IsWinner = _diceRoller.Doubles.ToString() == guest ? true : false;
                gambleResultsText.text = $"{(_diceRoller.Doubles ? "是雙骰!" : "不是雙骰!")}，{(_diceRoller.IsWinner ? "你贏了!!!" : "你輸了...")}";
                break;
        }
        if (_diceRoller.IsWinner)
        {
            DataManager.Instance.MoneyCount += (int)(betMoneyCount * currentOdds);
        }
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
        exitButton.gameObject.SetActive(true);
        exitButton.onClick.AddListener(() =>
        {
            BattleManager.Instance.NextLevel("DiceRollerUI");
            UIManager.Instance.HideUI("UIShop");
            exitButton.gameObject.SetActive(false);
            exitButton.onClick.RemoveAllListeners();
        });
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
        {
            return;
        }

        gambleTypeTrans.gameObject.SetActive(true);
        betMoneyTrans.gameObject.SetActive(false);
        DataManager.Instance.MoneyCount -= amount;
        betMoneyCount = amount;
        title.text = "請選擇賭法";
        EventManager.Instance.DispatchEvent(EventDefinition.eventRefreshUI);
    }

    private void SumBet()
    {
        ShowBetType(sumBetTrans, GambleType.SumBet);
        title.text = "大或小";
    }

    private void DoublesBet()
    {
        ShowBetType(doublesBetTrans, GambleType.DoublesBet);
        title.text = "是否為雙骰";
    }

    private void ShowBetType(Transform betType, GambleType gambleType)
    {
        betType.gameObject.SetActive(true);
        gambleTypeTrans.gameObject.SetActive(false);
        currentType = gambleType;
    }
}
