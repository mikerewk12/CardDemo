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
        }

        public static class FaceCards
        {
            public static string Ace = "A";
            public static string King = "K";
            public static string Queen = "Q";
            public static string Jack = "J";
        }
    }
}
