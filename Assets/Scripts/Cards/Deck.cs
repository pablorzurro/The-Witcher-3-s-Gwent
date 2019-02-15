using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour {

    [SerializeField] private List<GameObject> deck; // Rest of the deck that are not hand cards. 
   
    void Awake()
    {
       deck = ShuffleDeck(deck);
    }

    /// <summary>
    /// Shuffles a input list of gameobjects.
    /// </summary>
    /// <param name="deckTemp"></param>
    /// <returns></returns>
    List<GameObject> ShuffleDeck(List<GameObject> deckTemp)
    {
        for (int i = 0; i < deckTemp.Count; i++)
        {
            GameObject temp = deckTemp[i];
            int randomIndex = Random.Range(i, deckTemp.Count);
            deckTemp[i] = deckTemp[randomIndex];
            deckTemp[randomIndex] = temp;
        }
        return deckTemp;
    }

    /// <summary>
    /// Steals the according number of cards from this deck and adds it to hand.
    /// </summary>
    /// <param name="nCardsToSteal"></param>
    /// <param name="nHandCardsLimit"></param>
    /// <param name="hand"></param>
    public void StealNCardsFromDeck(int nCardsToSteal, int nHandCardsLimit, GameObject hand)
    {
        int limit = nHandCardsLimit - hand.transform.childCount;

        if(limit > deck.Count)
        {
            limit = deck.Count;
        }
        for (int i = 0; i < nCardsToSteal && i < limit ; i++) {
            GameObject temp = Instantiate(deck[0], hand.transform);
            temp.GetComponent<Card>().player = GetComponent<Player>();
            deck.RemoveAt(0);
        }   
    }


}
