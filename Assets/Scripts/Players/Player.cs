using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    //Select in its turn the card from its hand 
    // Selection from raycast & IA evaluation 
    // Selection of field when selected boost card &  IA evaluation 

    Card lastSelectedCard = null;
    Card confirmatedCard = null;

    public GameObject hand;

    [SerializeField] GameController gameController;
    [SerializeField] protected PJBoard_Manager boardPlayer;
    [SerializeField] protected GameObject fieldsParent;
    [SerializeField] protected Text scoreText;
    [SerializeField] protected Text handCardsNumber;

    [System.NonSerialized] public int score;
    [System.NonSerialized] public bool givesUp;
    [System.NonSerialized] public bool myTurn;
    [System.NonSerialized] protected bool moved;
    [System.NonSerialized] public byte winnedRounds;

    protected GameObject selectedCard;

    protected void Awake()
    {
        DefaultRoundAtributes();
        winnedRounds = 0;
    }

    protected void Start()
    {
        UpdateStats();
    }
    protected void Update()
    {
        CheckMove();
    }

    virtual protected void CheckMove()
    {
        if (myTurn)
        {
            bool isHandEmpty = IsHandEmpty();

            if (isHandEmpty)
            {
                moved = true;
                givesUp = true;
            }
            else
            {
                if (selectedCard == null)
                {
                    selectedCard = TurnLogic();
                }
            }

            if (moved)
            {
                ConfirmateMove();
                moved = false;
            }
        }
    }

    /// <summary>
    /// Sets the turn to a player
    /// </summary>
    public void SetTurn()
    {
        myTurn = true;
    }

    public bool IsMyTurn()
    {
        return myTurn;
    }

    /// <summary>
    /// Calls the player's board manager to add a card to the board, update player's score and reset values to default.
    /// </summary>
    protected void ConfirmateMove()
    {
        if (!givesUp)
        {
            boardPlayer.AddCard(selectedCard);
        }

        Debug.Log("Movimiento confirmado   " + this);
        UpdateStats();
        if (!givesUp) { ResetTurnConditions(); }
        

        gameController.NextTurn();
    }

    #region Auxiliar Methods
    /// <summary>
    /// Update the score and number of cards in hand (also updates the texts).
    /// </summary>
    protected void UpdateStats()
    {
        score = boardPlayer.GetTotalScore();
        scoreText.text = score.ToString();

        handCardsNumber.text = hand.transform.childCount.ToString();
    }

    /// <summary>
    /// Reset the round conditions (all atributes except winned rounds).
    /// </summary>
    protected void DefaultRoundAtributes()
    {
        givesUp = false;
        score = 0;
        ResetTurnConditions();
    }


    /// <summary>
    /// Reset all turn conditions to it's default values (is not the turn of this player and has not selected a card).
    /// </summary>
    protected void ResetTurnConditions()
    {
        myTurn = false;
        moved = false;
        selectedCard = null;
    }

    /// <summary>
    /// Resets everything except winned rounds and the cards on the player's deck. 
    /// </summary>
    /// <param name="cardsToSteal"></param>
    /// <param name="maxCardsInHand"></param>
    public void RoundReset(int cardsToSteal, int maxCardsInHand)
    {
        DefaultRoundAtributes();
        ResetTurnConditions();
        boardPlayer.ClearBoard();

        StealCards(cardsToSteal, maxCardsInHand);
    }

    /// <summary>
    /// Return a boolean with the emptyness condition of the player's hand. 
    /// </summary>
    /// <returns></returns>
    public bool IsHandEmpty()
    {
        return hand.transform.childCount == 0;
    }

    public void StealCards(int cardsToSteal, int maxCardsInHand)
    {

        GetComponent<Deck>().StealNCardsFromDeck(cardsToSteal, maxCardsInHand, hand);
    }
    #endregion

  
    /// <summary>
    /// Returns the selectedCardObj and checks if the player is surrending. 
    /// </summary>
    /// <returns></returns>
    virtual protected GameObject TurnLogic()
    {
        GameObject selectedCardObj = PickCard(); // Picking card...

        MakeHandInteractable(true);

        IsSurrending();

        if(selectedCardObj != null) // Card selected 
        {
            moved = true;
            MakeHandInteractable(false);
        }
        else if (givesUp && !moved)  // Has surrended
        {
            moved = true; 
            MakeHandInteractable(false); //The hand returns to a non interactable state. 
        }
       return selectedCardObj;
    }

    #region Card Selection and Interaction
    /// <summary>
    /// Core of the card selection to human players. Returns the selected card object that have been confirmated. The confirmation needs 2 clicks to the same object or if is a Boost Card (Commander's Horn) choose the line of affliction.
    /// </summary>
    /// <returns></returns>
    virtual protected GameObject PickCard()
    {
        GameObject tempCardObj = null;

        if (lastSelectedCard != null)
        {
            if (lastSelectedCard.GetEffect() == ScoreEffects.effect.x2)
            {
                EnableRowSelection(boardPlayer.GetUnaffectedRows(), fieldsParent); // Enables the row selection for the Commander's Horn card with the unnaffected rows (if a CH card is selected)

                if (boardPlayer.GetSelectedRow() != FieldEnum.fields.undefined)
                {
                    lastSelectedCard.SetField(boardPlayer.GetSelectedRow()); // Sets the destination field of the CH card. 
                    boardPlayer.SetSelectedRow(3); // Return to undefined state. 
                    confirmatedCard = lastSelectedCard; // Card has been confirmed
                }
            }
            else
            {
                DisableAllRows(fieldsParent);
            }
        }
        
        if(confirmatedCard != null)
        {
            DisableAllRows(fieldsParent);
            tempCardObj = confirmatedCard.gameObject; // Object of return with the confirmated card value. 
            lastSelectedCard = null;
            confirmatedCard = null;
        }
        return tempCardObj;
    } 

    /// <summary>
    /// Method that is called by all the pressed cards on the hand. Gives info of which is the last card selected and if is not a CH card that has been pressed 2 times sets it the confirmatedCard. Also recives the information of the Card thas calls this method. 
    /// </summary>
    /// <param name="inputCard"></param>
    public void SetSelectedCard(Card inputCard)
    {
        confirmatedCard = null;

        if (lastSelectedCard != null)
        {
            if (lastSelectedCard.gameObject == inputCard.gameObject) // Card has been selected previously. 
            {
                if (inputCard.GetEffect() != ScoreEffects.effect.x2)
                {
                    confirmatedCard = inputCard;  
                }
            }
            else
            {
                lastSelectedCard = inputCard;
            }
        }
        else
        {
            lastSelectedCard = inputCard;
        }
    }

    /// <summary>
    /// Disable or enable the interactability of the hand  
    /// </summary>
    /// <param name="interactability"></param>
    private void MakeHandInteractable(bool interactability)
    {
        for (int i = 0; i < hand.transform.childCount; i++)
        {
            hand.transform.GetChild(i).GetComponent<Card>().MakeInteractable(interactability);
        }
        if (!interactability)
        {
            DisableAllRows(fieldsParent);
        }
    }
    /**
    * Method to enable rows for Commander's Horn line selection. 
    */
    private void EnableRowSelection(List<int> availableRows, GameObject sceneParent)
    {
        foreach (int row in availableRows)
        {
            GameObject rowButton = sceneParent.transform.GetChild(row).gameObject;
            rowButton.GetComponent<Button>().enabled = true;
            rowButton.GetComponent<Image>().enabled = true;
        }
    }

    /// <summary>
    /// Disables Commander's Horn line selection. 
    /// </summary>
    /// <param name="sceneParent"></param>
    private void DisableAllRows(GameObject sceneParent)
    {
        int limit = sceneParent.transform.childCount - 1; // Hand is not necessary, we have the following rows with button component: Closed Combat, Ranged Combat and Siege Combat
        for (int i = 0; i < limit; i++)
        {
            GameObject rowButton = sceneParent.transform.GetChild(i).gameObject;
            rowButton.GetComponent<Button>().enabled = false;
            rowButton.GetComponent<Image>().enabled = false;
        }
    }
    #endregion

    #region Surrender 
    /// <summary>
    /// Returns a boolean with the surrending condition of the player.
    /// </summary>
    /// <returns></returns>
    private void IsSurrending()
    {
        if (Input.GetKey(KeyCode.G) && Input.GetKey(KeyCode.U) && !givesUp)
        {
            givesUp = true;
        }
    }

    public bool HasSurrended()
    {
        return givesUp;
    }
    #endregion

}