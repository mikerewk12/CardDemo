using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Assets.Scripts.Constants;

public class Card : MonoBehaviour
{
    private const int SUIT_INDEX = 1;
    private const int STRING_INDEX = 2;

    public string Suit;
    public string StringValue;
    public int IntValue;

    void Awake()
    {
        // prebab naming convention is: Card_club_A
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
                if (StringValue == FaceCards.Ace)
                {
                    IntValue = 1;
                }
                else if (StringValue == FaceCards.King || StringValue == FaceCards.Queen || StringValue == FaceCards.Jack)
                {
                    IntValue = 10;
                }
            }
            else if (sceneName == GameType.HighLow || sceneName == GameType.HighLowPoker)
            {
                if (StringValue == FaceCards.Ace)
                {
                    IntValue = 1;
                }
                else if (StringValue == FaceCards.King)
                {
                    IntValue = 13;
                }
                else if (StringValue == FaceCards.Queen)
                {
                    IntValue = 12;
                }
                else if (StringValue == FaceCards.Jack)
                {
                    IntValue = 11;
                }
            }
            else if (sceneName == GameType.Poker)
            {
                if (StringValue == FaceCards.Ace)
                {
                    IntValue = 14;
                }
                else if (StringValue == FaceCards.King)
                {
                    IntValue = 13;
                }
                else if (StringValue == FaceCards.Queen)
                {
                    IntValue = 12;
                }
                else if (StringValue == FaceCards.Jack)
                {
                    IntValue = 11;
                }
            } 
            else
            {
                throw new Exception("Game type not found. Define a new game type and add it to Card so we can determine the values properly.");
            }
        }
    }
}
