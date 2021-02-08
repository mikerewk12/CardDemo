using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Assets.Scripts.Constants;

namespace Assets.Scripts
{
    public class Dealer_HighLow : DealerBase
    {
        private int WinStreak = 0;
        private const int PAYOUT_WINS = 5;
        private int Multiplier = 2;
        public TextMeshProUGUI txtWinStreak;
        public TextMeshProUGUI txtMultiplier;
        static class Positions
        {
            public static int Start = 1;
            public static int End = 2;
            public static int Middle = 3;
            public static int Left = 4;
            public static int Right = 5;
        }

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
        }

        public override Dictionary<int, Vector3> CardPositions => new Dictionary<int, Vector3>()
        {
            { Positions.Start, new Vector3(7.71f, 3.5f, 0) },
            { Positions.End, new Vector3(-15, -5, 0) },
            { Positions.Middle, new Vector3(0.04f, 0, 0) },
            { Positions.Left, new Vector3(-1.5f, 0, 0) },
            { Positions.Right, new Vector3(1.5f, 0, 0) }
        };

        public override void BeginNewTurn()
        {
            // reshuffle the deck if there are no more cards left
            if(DeckOfCards.CardsRemaining == 1)
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
            yield return new WaitForSeconds(2f);

            Card PlayerCard = Player.Hand[0];
            Card DealerCard = DeckOfCards.NextCard(CardPositions[Positions.Start]);
            bool playerWins = false;

            // move the players card into next position
            PlayerCard.transform.DOMove(CardPositions[Positions.Left], 1);

            // give the dealer a card and move it position
            DealerCard.transform.DOMove(CardPositions[Positions.Right], 1);
            this.Hand.Add(DealerCard);

            yield return new WaitForSeconds(2f);

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
                // TODO: some cool animation here
                WinStreak++;

                // give payouts every 5 consecutive wins
                if ((WinStreak % PAYOUT_WINS) == 0)
                {
                    // TODO: another cool animation
                    float winnings = PotTotal * Multiplier;
                    Player.ReceivePayout(winnings);
                    UpdateMultiplier(Multiplier + 1);
                    UpdatePot(0);
                    StartCoroutine(ShowOutput($"Payout of ${winnings}!!"));
                }
                else
                {
                    StartCoroutine(ShowOutput($"GOOD ONE"));
                }
            }
            else
            {
                WinStreak = 0;
                UpdateMultiplier(2);
                UpdatePot(0);
                StartCoroutine(ShowOutput($"Oh no :("));
            }

            DealerCard.transform.DOMove(CardPositions[Positions.Middle], 1);
            PlayerCard.transform.DOMove(CardPositions[Positions.End], 1);
            Player.ReceiveNewHand(this.Hand);
            this.ReceiveNewHand(new List<Card>());
            txtWinStreak.text = $"Wins in a row: {WinStreak}";
        }

        private void UpdateMultiplier(int newValue)
        {
            Multiplier = newValue;
            txtMultiplier.text = $"Multiplier: {Multiplier}";
        }
    }
}
