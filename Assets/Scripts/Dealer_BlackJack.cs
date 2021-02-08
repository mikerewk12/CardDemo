using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Dealer_BlackJack : DealerBase
{
    public TextMeshProUGUI txtPlayerTotal;
    public TextMeshProUGUI txtDealerTotal;
    static class Positions
    {
        public static int Start = 0;

        public static int PlayerLeft = 1;
        public static int PlayerRight = 2;
        public static int PlayerExtraStart = 3;

        public static int DealerLeft = 4;
        public static int DealerRight = 5;
        public static int DealerExtraStart = 6;
    }

    static class Actions
    {
        public static int HitMe = 1;
        public static int Stay = 2;
    }

    public override Dictionary<int, Vector3> CardPositions => new Dictionary<int, Vector3>()
    {
        { Positions.Start, new Vector3(7.71f, 3.5f, 0) },
        { Positions.PlayerLeft, new Vector3(-1.2f, -2, 0) },
        { Positions.PlayerRight, new Vector3(1.17f, -2, 0) },
        { Positions.PlayerExtraStart, new Vector3(3.34f, -2, 0) },
        { Positions.DealerLeft, new Vector3(-1.21f, 2, 0) },
        { Positions.DealerRight, new Vector3(1.17f, 2, 0) },
        { Positions.DealerExtraStart, new Vector3(-3.34f, 2, 0) }
    };

    public override void Start()
    {
        DeckOfCards.Shuffle();
        BeginNewTurn();
        UpdatePot(0);
    }
    public override void BeginNewTurn()
    {
        StartCoroutine(Deal());
    }

    private IEnumerator Deal()
    {
        UpdatePlayerTotal(0);
        UpdateDealerTotal(0);

        // reshuffle the deck if we might run out of cards
        if (DeckOfCards.CardsRemaining < 15)
        {
            DeckOfCards.Shuffle();
        }

        // take the bet from the player and put it into the pot
        if (!TakeBetFromPlayer())
        {
            StartCoroutine(ShowOutput("Insufficient funds"));
            yield break;
        }

        Card playerCard1 = DeckOfCards.NextCard(CardPositions[Positions.Start]);
        playerCard1.transform.DOMove(CardPositions[Positions.PlayerLeft], 1);
        yield return new WaitForSeconds(.5f);

        Card playerCard2 = DeckOfCards.NextCard(CardPositions[Positions.Start]);
        playerCard2.transform.DOMove(CardPositions[Positions.PlayerRight], 1);
        yield return new WaitForSeconds(.5f);

        Player.ReceiveNewHand(new List<Card>() { playerCard1, playerCard2 });
        UpdateAce(Player.Hand);
        UpdatePlayerTotal(playerCard1.IntValue + playerCard2.IntValue);

        Card dealerCard1 = DeckOfCards.NextCard(CardPositions[Positions.Start], false);
        dealerCard1.transform.DOMove(CardPositions[Positions.DealerLeft], 1);
        yield return new WaitForSeconds(.5f);

        Card dealerCard2 = DeckOfCards.NextCard(CardPositions[Positions.Start]);
        dealerCard2.transform.DOMove(CardPositions[Positions.DealerRight], 1);

        ReceiveNewHand(new List<Card>() { dealerCard1, dealerCard2 });
        
    }
    
    public override void ProcessPlayerAction(int action)
    {
        StartCoroutine(TakeAction(action));
    }

    private IEnumerator TakeAction(int action)
    {
        // TODO: show pot increasing and bank roll decreasing animation
        yield return new WaitForSeconds(1f);

        if (action == Actions.HitMe)
        {
            // give player another card
            Card newPlayerCard = DeckOfCards.NextCard(CardPositions[Positions.Start]);

            // figure out the placement of the next card
            int howMany = Player.Hand.Count - 2;
            float nextX = CardPositions[Positions.PlayerExtraStart].x + (.5f * howMany);
            float nextZ = CardPositions[Positions.PlayerExtraStart].z + (-.5f * howMany);
            Vector3 nextCardPosition = new Vector3(nextX, CardPositions[Positions.PlayerRight].y, nextZ);

            newPlayerCard.transform.DOMove(nextCardPosition, 1);
            Player.Hand.Add(newPlayerCard);

            yield return new WaitForSeconds(1);

            // get the total value of the cards in the players hand and check for bust
            UpdateAce(Player.Hand);
            int totalPlayer = Player.Hand.Select(m => m.IntValue).Sum();
            UpdatePlayerTotal(totalPlayer);

            if (totalPlayer > 21)
            {
                // bust
                StartCoroutine(DealNewGame("You BUST!"));
            }

        }
        else if (action == Actions.Stay)
        {
            int totalPlayer = Player.Hand.Select(m => m.IntValue).Sum();


            StartCoroutine(DealerTurn());
        }
    }
    
    private IEnumerator DealerTurn()
    {
        // flip hidden card over
        Transform cardToFlip = this.Hand[0].GetComponent<Transform>();
        Quaternion flipQuat = Quaternion.Euler(270, cardToFlip.rotation.y, cardToFlip.rotation.z);
        cardToFlip.rotation = Quaternion.Lerp(cardToFlip.rotation, flipQuat, Time.time * .5f);

        // get the total value of the cards in each players hand
        int totalPlayer = Player.Hand.Select(m => m.IntValue).Sum();
        int totalDealer = this.Hand.Select(m => m.IntValue).Sum();

        UpdateDealerTotal(totalDealer);

        yield return new WaitForSeconds(1);

        // keep giving dealer a card until bust or beats player
        while(totalDealer < totalPlayer && totalDealer < 21)
        {
            // figure out the placement of the next card
            int howMany = this.Hand.Count - 2;
            float nextX = CardPositions[Positions.DealerExtraStart].x + (-.5f * howMany);
            float nextZ = CardPositions[Positions.DealerExtraStart].z + (.5f * howMany);
            Vector3 nextCardPosition = new Vector3(nextX, CardPositions[Positions.DealerExtraStart].y, nextZ);

            // give dealer new card
            Card nextCard = DeckOfCards.NextCard(nextCardPosition);
            this.Hand.Add(nextCard);

            UpdateAce(Hand);

            // update dealer total
            totalDealer += nextCard.IntValue;
            UpdateDealerTotal(totalDealer);

            yield return new WaitForSeconds(1);
        }

        if(totalDealer > 21)
        {
            // win double the pot
            Player.ReceivePayout(PotTotal * 2);
            StartCoroutine(DealNewGame("Dealer Bust - You win!"));
        }
        else
        {
            StartCoroutine(DealNewGame("Dealer wins - You lose :("));
        }
    }

    private IEnumerator DealNewGame(string message)
    {
        StartCoroutine(ShowOutput(message));
        yield return new WaitForSeconds(2);
        UpdatePot(0);

        foreach(Card card in this.Hand)
        {
            Destroy(card.gameObject);
        }
        this.Hand.Clear();

        foreach(Card card in Player.Hand)
        {
            Destroy(card.gameObject);
        }
        Player.Hand.Clear();
        StartCoroutine(Deal());
    }

    private void UpdatePlayerTotal(int newTotal)
    {
        txtPlayerTotal.text = newTotal.ToString();
    }

    private void UpdateDealerTotal(int newTotal)
    {
        txtDealerTotal.text = newTotal.ToString();
    }

    /// <summary>
    /// Checks for an ace in the hand and flips the value of the card to 11 if it can take it
    /// </summary>
    /// <param name="hand"></param>
    private void UpdateAce(List<Card> hand)
    {
        // TODO: find a better way to do this
        int total = hand.Select(m => m.IntValue).Sum();
        Card ace = hand.Where(m => m.StringValue == "A").FirstOrDefault();

        if (ace != null)
        {
            if ((total + 10) <= 21)
            {
                ace.IntValue = 11;
            }
            else
            {
                ace.IntValue = 1;
            }
        }
    }
}
