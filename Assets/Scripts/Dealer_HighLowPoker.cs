using Assets.Scripts;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Dealer_HighLowPoker : DealerBase
{
    private int WinStreak = 0;
    private const int MAX_WIN_STREAK = 5;
    private List<Card> PokerHand = new List<Card>();
    public TextMeshProUGUI txtWinStreak;
    public TextMeshProUGUI txtHandRank;
    public TextMeshProUGUI txtTotalWinnings;
    public Canvas PokerHandCanvas;
    public ParticleSystem Rain;
    public AudioListener AudioListener;
    public GameObject Background;
    public AudioClip AudioGood;
    public AudioClip AudioBad;
    public AudioClip AudioCheer;
    static class Positions
    {
        public static int Start = 1;
        public static int End = 2;
        public static int Middle = 3;
        public static int Left = 4;
        public static int Right = 5;

        public static int PokerHand1 = 6;
        public static int PokerHand2 = 7;
        public static int PokerHand3 = 8;
        public static int PokerHand4 = 9;
        public static int PokerHand5 = 10;

        public static int PokerHandDisplay1 = 11;
        public static int PokerHandDisplay2 = 12;
        public static int PokerHandDisplay3 = 13;
        public static int PokerHandDisplay4 = 14;
        public static int PokerHandDisplay5 = 15;
    }
    public override Dictionary<int, Vector3> CardPositions => new Dictionary<int, Vector3>()
        {
            { Positions.Start, new Vector3(7.71f, 3.5f, -1) },
            { Positions.End, new Vector3(-15, -5, -1) },

            { Positions.Middle, new Vector3(0.04f, 0, -1) },
            { Positions.Left, new Vector3(-1.5f, 0, -1) },
            { Positions.Right, new Vector3(1.5f, 0, -1) },

            { Positions.PokerHandDisplay1, new Vector3(-6f, 0f, -1) },
            { Positions.PokerHandDisplay2, new Vector3(-3f, 0, -1) },
            { Positions.PokerHandDisplay3, new Vector3(0, 0, -1) },
            { Positions.PokerHandDisplay4, new Vector3(3f, 0, -1) },
            { Positions.PokerHandDisplay5, new Vector3(6f, 0, -1) },

            { Positions.PokerHand1, new Vector3(-7.5f, -2.7f, -1) },
            { Positions.PokerHand2, new Vector3(-6.5f, -2.7f, -2) },
            { Positions.PokerHand3, new Vector3(-5.5f, -2.7f, -3) },
            { Positions.PokerHand4, new Vector3(-4.5f, -2.7f, -4) },
            { Positions.PokerHand5, new Vector3(-3.5f, -2.7f, -5) }
        };

    static class Actions
    {
        public static int BetHigh = 1;
        public static int BetLow = 2;
    }

    public override void Start()
    {
        DeckOfCards.Shuffle();
        BeginNewTurn();
        UpdatePot(0);
        Rain.Stop();
    }

    public override void BeginNewTurn()
    {
        // reshuffle the deck if there are no more cards left
        if (DeckOfCards.CardsRemaining == 1)
        {
            DeckOfCards.Shuffle(new List<string>() { Player.Hand[0].name });
        }
        // get the next card from the deck
        Card nextCard = DeckOfCards.NextCard(CardPositions[Positions.Start]);

        // give it to the player
        Player.ReceiveNewHand(new List<Card>() { nextCard });

        // move it into position
        nextCard.transform.DOMove(CardPositions[Positions.Middle], 1);
    }

    public override void ProcessPlayerAction(int action)
    {
        StartCoroutine(TakeAction(action));
    }

    private IEnumerator TakeAction(int action)
    {
        // take the bet from the player and put it into the pot
        if (!TakeBetFromPlayer())
        {
            yield break;
        }

        // TODO: show pot increasing and bank roll decreasing animation
        yield return new WaitForSeconds(1f);

        Card PlayerCard = Player.Hand[0];
        Card DealerCard = DeckOfCards.NextCard(CardPositions[Positions.Start]);
        bool playerWins = false;

        // move the players card into next position
        PlayerCard.transform.DOMove(CardPositions[Positions.Left], 1);

        // give the dealer a card and move it position
        DealerCard.transform.DOMove(CardPositions[Positions.Right], 1);
        this.Hand.Add(DealerCard);

        yield return new WaitForSeconds(1f);

        // do comparision to see if the player was correct
        if (action == Actions.BetHigh)
        {
            playerWins = (PlayerCard.IntValue < DealerCard.IntValue) ? true : false;
        }
        else if (action == Actions.BetLow)
        {
            playerWins = (PlayerCard.IntValue > DealerCard.IntValue) ? true : false;
        }

        if (playerWins)
        {
            WinStreak++;
            this.GetComponent<AudioSource>().PlayOneShot(AudioGood);

            // put card into the next poker hand slot
            int nextSlotIndex = GetNextPokerHandPosition();
            PlayerCard.transform.DOMove(CardPositions[nextSlotIndex], 1);
            PokerHand.Add(PlayerCard);

            if ((WinStreak % MAX_WIN_STREAK) == 0)
            {
                // player guessed 5 in a row
                StartCoroutine(PlayPoker());
            }
            else
            {
                StartCoroutine(ShowOutput($"GOOD ONE"));
            }
        }
        else
        {
            WinStreak = 0;
            UpdatePot(0);
            this.GetComponent<AudioSource>().PlayOneShot(AudioBad);
            StartCoroutine(ShowOutput($"Oh no :("));
            PlayerCard.transform.DOMove(CardPositions[Positions.End], 1);
            // clear any cards in poker hand slots
            ClearPokerHand();
        }

        DealerCard.transform.DOMove(CardPositions[Positions.Middle], 1);
        
        Player.ReceiveNewHand(this.Hand);
        this.ReceiveNewHand(new List<Card>());
        txtWinStreak.text = $"Wins in a row: {WinStreak}";
    }

    IEnumerator PlayPoker()
    {
        // wait for a sec
        yield return new WaitForSeconds(1f);

        // hide deck and new player card for now
        Card playerCard = Player.Hand[0];
        playerCard.transform.position = new Vector3(playerCard.transform.position.x, playerCard.transform.position.y, -10);
        DeckOfCards.transform.position = new Vector3(DeckOfCards.transform.position.x, DeckOfCards.transform.position.y, -10);

        // enable other canvas layer to hide UI layer
        PokerHandCanvas.enabled = true;
        Rain.Play();
        Background.SetActive(false);
        this.GetComponent<AudioSource>().PlayOneShot(AudioCheer);

        // move cards
        for (int i = 0; i < 5; i++)
        {
            Card card = PokerHand[i];
            switch (i)
            {
                case 0:
                    card.transform.DOMove(CardPositions[Positions.PokerHandDisplay1], 1);
                    break;
                case 1:
                    card.transform.DOMove(CardPositions[Positions.PokerHandDisplay2], 1);
                    break;
                case 2:
                    card.transform.DOMove(CardPositions[Positions.PokerHandDisplay3], 1);
                    break;
                case 3:
                    card.transform.DOMove(CardPositions[Positions.PokerHandDisplay4], 1);
                    break;
                case 4:
                    card.transform.DOMove(CardPositions[Positions.PokerHandDisplay5], 1);
                    break;
            }
        }

        yield return new WaitForSeconds(1f);

        // figure out the score of the hand
        PokerHand handResults = new PokerHand(PokerHand);
        txtHandRank.text = $"You got a {handResults.RankString()} worth a {handResults.BaseScore}x multiplier!";

        yield return new WaitForSeconds(2f);

        float winnings = (PotTotal * handResults.BaseScore);
        txtTotalWinnings.text = $"Pot {PotTotal} X {handResults.BaseScore} = ${winnings}";
        Player.UpdateBankRoll(Player.BankRoll + winnings);
        UpdatePot(0);
        Rain.Stop();

    }

    private int GetNextPokerHandPosition()
    {
        switch (WinStreak)
        {
            case 1:
                return Positions.PokerHand1;
            case 2:
                return Positions.PokerHand2;
            case 3:
                return Positions.PokerHand3;
            case 4:
                return Positions.PokerHand4;
            case 5:
                return Positions.PokerHand5;
            default:
                throw new Exception("Win streak exceeded available poker hand slots. You didn't reset the win streak variable?");
        }
    }

    public void ContinueAfterPoker()
    {
        // hide the canvas and clear text
        PokerHandCanvas.enabled = false;
        txtHandRank.text = "";
        txtTotalWinnings.text = "";
        Background.SetActive(true);

        // clear the poker hand
        ClearPokerHand();

        // reset win streak
        WinStreak = 0;
        txtWinStreak.text = $"Wins in a row: {WinStreak}";

        // return card and deck to original z
        Card playerCard = Player.Hand[0];
        playerCard.transform.position = new Vector3(playerCard.transform.position.x, playerCard.transform.position.y, -1);
        DeckOfCards.transform.position = new Vector3(DeckOfCards.transform.position.x, DeckOfCards.transform.position.y, 0);

    }

    private void ClearPokerHand()
    {
        foreach (Card card in PokerHand)
        {
            card.transform.DOMove(CardPositions[Positions.End], 1);
        }
        PokerHand.Clear();
    }
}
