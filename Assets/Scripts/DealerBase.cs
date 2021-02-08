using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Assets.Scripts.Constants;

public abstract class DealerBase : MonoBehaviour
{
    public Deck DeckOfCards;
    public Player Player;
    public TextMeshProUGUI txtPotTotal;
    public TextMeshProUGUI txtOutput;

    protected List<Card> Hand = new List<Card>();
    protected float PotTotal = 0;

    public abstract void Start();
    public abstract void BeginNewTurn(); // Deal the cards and wait for a response
    public abstract void ProcessPlayerAction(int action); // When the player takes an action
    public abstract Dictionary<int, Vector3> CardPositions { get; }

    /// <summary>
    /// Removes bet amount from players bank roll and adds it to the pot
    /// </summary>
    /// <returns>True if the player had the funds</returns>
    public bool TakeBetFromPlayer()
    {
        if((Player.BankRoll - Player.BetAmount) < 0)
        {
            StartCoroutine(ShowOutput("Insufficient funds"));
            return false;
        }
        else
        {
            UpdatePot(PotTotal + Player.MakeBet());
            return true;
        }
    }

    public void ReceiveNewHand(List<Card> newHand)
    {
        Hand = newHand;
    }

    public void UpdatePot(float newAmount)
    {
        PotTotal = newAmount;
        txtPotTotal.text = $"Pot is {PotTotal}";
    }

    public IEnumerator ShowOutput(string message)
    {
        txtOutput.text = message;
        yield return new WaitForSeconds(03);
        txtOutput.text = string.Empty;
    }
}
