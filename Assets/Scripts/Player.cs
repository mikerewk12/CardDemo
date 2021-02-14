using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Assets.Scripts.Constants;

public class Player : MonoBehaviour
{
    public TextMeshProUGUI txtBankRoll;
    public TextMeshProUGUI txtBetAmount;
    public float BankRoll = 1000;
    public float BetAmount = 20;
    public List<Card> Hand = new List<Card>();

    private void Start()
    {
        UpdateBankRoll(BankRoll);
        UpdateBetAmount(BetAmount);
    }

    public void BetAmountUp()
    {
        float newValue = BetAmount * 2;
        if(newValue <= MAX_BET_AMOUNT)
        {
            UpdateBetAmount(BetAmount * 2);
        }
    }

    public void BetAmountDown()
    {
        float newValue = BetAmount / 2;
        if(newValue >= MIN_BET_AMOUNT)
        {
            UpdateBetAmount(BetAmount / 2);
        }
    }

    private void UpdateBetAmount(float newAmount)
    {
        BetAmount = newAmount;
        txtBetAmount.text = $"${BetAmount}";
    }

    public float MakeBet()
    {
        UpdateBankRoll(BankRoll - BetAmount);
        return BetAmount;
    }

    public void ReceiveNewHand(List<Card> newHand)
    {
        Hand = newHand;
    }

    public void ReceivePayout(float payout)
    {
        UpdateBankRoll(BankRoll + payout);
    }

    public void UpdateBankRoll(float newAmount)
    {
        // TODO: some cool animation
        BankRoll = newAmount;
        txtBankRoll.text = $"Bank Roll ${BankRoll.ToString()}";
    }
}
