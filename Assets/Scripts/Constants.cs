using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public static class Constants
    {
        public const float MAX_BET_AMOUNT = 200;
        public const float MIN_BET_AMOUNT = 5;

        public static class GameType
        {
            public static string HighLow = "highlow";
            public static string BlackJack = "blackjack";
            public static string Poker = "poker";
            public static string HighLowPoker = "highlowpoker";
        }

        public static class FaceCards
        {
            public static string Ace = "A";
            public static string King = "K";
            public static string Queen = "Q";
            public static string Jack = "J";
        }

        public enum HandRanking
        {
            High_Card = 2,
            Pair = 3,
            Two_Pair = 5,
            Three_Of_A_Kind = 10,
            Straight = 15,
            Flush = 20,
            Full_House = 25,
            Four_Of_A_Kind = 50,
            Straight_Flush = 75,
            Royal_Flush = 100
        }
    }

}
