using UnityEngine;
using System.Collections.Generic;
using TMPro;

namespace Assets.Scripts
{
    /// <summary>
    /// A deck of cards for a regular 52 card deck
    /// </summary>
    public class Deck : MonoBehaviour
    {
        public TextMeshProUGUI txtNextCard;
        public int CardsRemaining { get { return _shuffledCardNames.Count; } }

        // a list of all card names so we only load the cards into the scene as they're needed (mobile first habit)
        private List<string> _shuffledCardNames;

        public void Shuffle(List<string> cardsInPlay = null)
        {
            List<string> cardNames = new List<string>()
            {
                // club
                "Card_club_2_",
                "Card_club_3_",
                "Card_club_4_",
                "Card_club_5_",
                "Card_club_6_",
                "Card_club_7_",
                "Card_club_8_",
                "Card_club_9_",
                "Card_club_10_",
                "Card_club_J_",
                "Card_club_Q_",
                "Card_club_K_",
                "Card_club_A_",
                // diamond
                "Card_diamond_2_",
                "Card_diamond_3_",
                "Card_diamond_4_",
                "Card_diamond_5_",
                "Card_diamond_6_",
                "Card_diamond_7_",
                "Card_diamond_8_",
                "Card_diamond_9_",
                "Card_diamond_10_",
                "Card_diamond_J_",
                "Card_diamond_Q_",
                "Card_diamond_K_",
                "Card_diamond_A_",
                // spade
                "Card_spade_2_",
                "Card_spade_3_",
                "Card_spade_4_",
                "Card_spade_5_",
                "Card_spade_6_",
                "Card_spade_7_",
                "Card_spade_8_",
                "Card_spade_9_",
                "Card_spade_10_",
                "Card_spade_J_",
                "Card_spade_Q_",
                "Card_spade_K_",
                "Card_spade_A_",
                // heart
                "Card_heart_2_",
                "Card_heart_3_",
                "Card_heart_4_",
                "Card_heart_5_",
                "Card_heart_6_",
                "Card_heart_7_",
                "Card_heart_8_",
                "Card_heart_9_",
                "Card_heart_10_",
                "Card_heart_J_",
                "Card_heart_Q_",
                "Card_heart_K_",
                "Card_heart_A_"
            };

            // shuffle the deck besides any cards in play
            if(cardsInPlay != null)
            {
                foreach(string cardInPlay in cardsInPlay)
                {
                    cardNames.Remove(cardInPlay);
                }
            }
            
            List<string> shuffledCardNames = new List<string>();
            for (var i = 0; i < 52; i++)
            {
                // get a random card based on the cards that are left and add it to the shuffled array
                int randomIndex = Random.Range(0, cardNames.Count - 1);
                shuffledCardNames.Add(cardNames[randomIndex]);
                cardNames.RemoveAt(randomIndex);
            }

            _shuffledCardNames = shuffledCardNames;
        }

        public Card NextCard(Vector3 startPosition, bool faceUp = true)
        {
            if(_shuffledCardNames.Count > 0)
            {
                string name = _shuffledCardNames[0];
                float faceRotation = (faceUp) ? 270 : 90;
                GameObject obj = (GameObject)Instantiate(Resources.Load($"Prefabs/Cards/{name}"), startPosition, Quaternion.Euler(faceRotation, 0, 0));
                _shuffledCardNames.RemoveAt(0);
                if(txtNextCard != null)
                {
                    txtNextCard.text = _shuffledCardNames[0];
                }
                return obj.GetComponent<Card>();
            }
            else
            {
                return null;
            }
        }
    }
}
