using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Assets.Scripts.Constants;

namespace Assets.Scripts
{
    public class PokerHand
    {

        public List<Card> Cards;
        public HandRanking Rank;
        public int BaseScore = 0;

        /// <summary>
        /// Figures out the score 
        /// </summary>
        /// <param name="selectedCards"></param>
        public PokerHand(List<Card> selectedCards)
        {
            // get the cards in order by int value
            Cards = selectedCards.OrderBy(m => m.IntValue).ToList();

            bool aceHigh = (Cards[4].IntValue == 14);
            bool flush = false;
            bool straight = false;
            bool fourOfAKind = false;
            bool threeOfAKind = false;
            bool twoPair = false;
            bool onePair = false;

            // flush
            flush = (Cards.Where(m => m.Suit == Cards[0].Suit).Count() == 5);

            // straight
            if (
                Cards[0].IntValue == Cards[1].IntValue - 1 &&
                Cards[1].IntValue == Cards[2].IntValue - 1 &&
                Cards[2].IntValue == Cards[3].IntValue - 1 &&
                Cards[3].IntValue == Cards[4].IntValue - 1
               )
            {
                straight = true;
            }
            else if ( // starts with a low ace
                Cards[0].IntValue == 2 &&
                Cards[1].IntValue == 3 &&
                Cards[2].IntValue == 4 &&
                Cards[3].IntValue == 5 &&
                Cards[4].IntValue == 14
               )
            {
                straight = true;
                aceHigh = false;
            }

            if (!flush && !straight)
            {
                // clone the list of cards
                Card[] cs = new Card[selectedCards.Count];
                selectedCards.CopyTo(cs);
                List<Card> temp = cs.ToList();

                while (temp.Count() > 0)
                {
                    // find all that match first card
                    List<Card> matches = temp.Where(m => m.IntValue == temp[0].IntValue).ToList();

                    // mark things as true for the number of matches and remove the card from temp array
                    if (matches.Count() == 4)
                    {
                        fourOfAKind = true;
                        temp.Clear();
                        break;
                    }
                    else if (matches.Count() == 3)
                    {
                        threeOfAKind = true;
                        for (var i = 0; i < matches.Count(); i++)
                        {
                            temp.Remove(matches[i]);
                        }
                    }
                    else if (matches.Count() == 2)
                    {
                        twoPair = (onePair == true) ? true : false;
                        onePair = true;
                        for (var i = 0; i < matches.Count(); i++)
                        {
                            temp.Remove(matches[i]);
                        }
                    }
                    else
                    {
                        temp.RemoveAt(0);
                    }
                }

            }

            if (straight && flush)
            {
                if (aceHigh)
                {
                    Rank = HandRanking.Royal_Flush;
                }
                else
                {
                    Rank = HandRanking.Straight_Flush;
                }
            }
            else if (fourOfAKind)
            {
                Rank = HandRanking.Four_Of_A_Kind;
            }
            else if (threeOfAKind && onePair)
            {
                Rank = HandRanking.Full_House;
            }
            else if (flush)
            {
                Rank = HandRanking.Flush;
            }
            else if (straight)
            {
                Rank = HandRanking.Straight;
            }
            else if (threeOfAKind)
            {
                Rank = HandRanking.Three_Of_A_Kind;
            }
            else if (twoPair)
            {
                Rank = HandRanking.Two_Pair;
            }
            else if (onePair)
            {
                Rank = HandRanking.Pair;
            }
            else
            {
                Rank = HandRanking.High_Card;
            }

            BaseScore = (int)Rank;

        }
        public string RankString()
        {
            return Rank.ToString().Replace("_", " ");
        }
    }
}
