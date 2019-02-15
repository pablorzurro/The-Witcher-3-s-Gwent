using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    [SerializeField] Player humanPlayer;
    [SerializeField] Player player2; //AI

    [SerializeField] Board_Manager boardManager;

    [SerializeField] int startCards;
    [SerializeField] int cardsToSteal;
    [SerializeField] int maxCardsInHand;

    int totalRounds;

    Player activePlayer;

    void Awake () {
        startCards = 10;
        cardsToSteal = 6 ;
        maxCardsInHand =  10;
        totalRounds = 0;
    }

    void Start()
    {
        NewRound(startCards, maxCardsInHand);  // New Round with first player
    }

    void Update()
    {
        
    }

    public void NextTurn()
    {
        bool isNewRound = HavePlayersSurrended();

        Debug.Log(HavePlayersSurrended());
        
        if (!isNewRound)
        {
            if (activePlayer != null)
            {
                if (!activePlayer.IsMyTurn())
                {
                    activePlayer.myTurn = false;
                    ChangePlayerTurn();
                    activePlayer.SetTurn();
                }
            }

        }
        else
        {
            Debug.Log(activePlayer);
            if (totalRounds < 3)
            {
                totalRounds++;
                NewRound(cardsToSteal, maxCardsInHand);
            }
        }
    }

    /// <summary>
    /// Returns true if the round has finished, checking if both players have surrended.
    /// </summary>
    /// <returns></returns>
    bool HavePlayersSurrended() {
        bool temp = false;

        if(humanPlayer.HasSurrended() && player2.HasSurrended())
        {
            humanPlayer.givesUp = player2.givesUp = false;
            ++GetRoundWinner().winnedRounds;
            temp = true;
        }

        return temp;
    }

    /// <summary>
    /// Sets the first turn to the input player received. 
    /// </summary>
    /// <param name="player"></param>
    void SetFirstTurn(Player player)
    {
        activePlayer = player;
        player.SetTurn();
    }

    /// <summary>
    /// Changes the turn with the next player that has to play.
    /// </summary>
    void ChangePlayerTurn()
    {
        activePlayer = GetNextPlayer(activePlayer);
        activePlayer.SetTurn();
    }

    /// <summary>
    /// Returns the next player available to play, if he has surrended calls the same function (recursivity) until find it.    
    /// </summary>
    /// <returns></returns>
    Player GetNextTurnPlayer()
    {
        Player tempTurnOwner = humanPlayer == activePlayer ? player2 : humanPlayer;

        if (tempTurnOwner.HasSurrended())
        {
            tempTurnOwner = GetNextPlayer(tempTurnOwner);
        }

        return tempTurnOwner;
    }



    /// <summary>
    /// Only returns one player or another depending on who is the active player. 
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    Player GetNextPlayer(Player player)
    {
        return humanPlayer == player ? player2 : humanPlayer;
    }

    /// <summary>
    /// Returns the player winner of a round.
    /// </summary>
    /// <returns></returns>
    Player GetRoundWinner()
    {
        return humanPlayer.score > player2.score ? humanPlayer : player2;
    }

    /// <summary>
    /// Resets all players round atributes and indicates them to steal cards with the instructions of the game controller rules. Also if is firts round gives the first turn to the human player and if not gives the first turn to the loser of the round. 
    /// </summary>
    /// <param name="nCardsToSteal"></param>
    /// <param name="maxNCardsInHand"></param>
    void NewRound(int nCardsToSteal, int maxNCardsInHand)
    {
        humanPlayer.RoundReset(nCardsToSteal, maxNCardsInHand);
        player2.RoundReset(nCardsToSteal ,maxNCardsInHand);

        if(activePlayer == null)
        {
            SetFirstTurn(humanPlayer);
        }
        else
        {
            SetFirstTurn(GetNextPlayer(GetRoundWinner()));
        }

        boardManager.UpdateBoardAndHands();
    }
}
