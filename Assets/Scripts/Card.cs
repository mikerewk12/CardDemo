using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Assets.Scripts.Constants;

public class Card : MonoBehaviour
{
    public const int SUIT_INDEX = 1;
    public const int STRING_INDEX = 2;

    public string Suit;
    public string StringValue;
    public int IntValue;

    void Awake()
    {
        string[] ar = gameObject.name.Split(char.Parse("_"));
        Suit = ar[SUIT_INDEX];
        StringValue = ar[STRING_INDEX];
        bool parsed = int.TryParse(ar[STRING_INDEX], out this.IntValue);

        // figure out face card values based on the game we're playing
        string sceneName = SceneManager.GetActiveScene().name.ToLower();
        if (!parsed)
        {
            if (sceneName == GameType.BlackJack)
            {
                if (StringValue == "A")
                {
                    IntValue = 1;
                }
                else if (StringValue == "K" || StringValue == "Q" || StringValue == "J")
                {
                    IntValue = 10;
                }
            }
            else if (sceneName == GameType.HighLow)
            {
                if (StringValue == "A")
                {
                    IntValue = 1;
                }
                else if (StringValue == "K")
                {
                    IntValue = 13;
                }
                else if (StringValue == "Q")
                {
                    IntValue = 12;
                }
                else if (StringValue == "J")
                {
                    IntValue = 11;
                }
            }
            else if (sceneName == GameType.Poker)
            {
                if (StringValue == "A")
                {
                    IntValue = 14;
                }
                else if (StringValue == "K")
                {
                    IntValue = 13;
                }
                else if (StringValue == "Q")
                {
                    IntValue = 12;
                }
                else if (StringValue == "J")
                {
                    IntValue = 11;
                }
            }
        }
    }
}
