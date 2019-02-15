using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Virtual_Board
{
    public static Field[] currentTurnPlayerBoard;
    public static Field[] currentTurnAIBoard;
    public static List<GameObject> currentHumanHand;
    public static List<GameObject> currentAIHand;

    Field[] player1SubBoard;
    bool[] handCardsAvailableP1;
    int scorePlayer1 = 0;

    Field[] player2SubBoard;
    bool[] handCardsAvailableP2;
    int scorePlayer2 = 0;


    private int actualPlayer;

    static PJBoard_Manager boardPlayerVirtual;

    public int GetScore() {
        if (boardPlayerVirtual.GetTotalScore(player1SubBoard) - boardPlayerVirtual.GetTotalScore(player2SubBoard) == 0) {
            return 1;
        }
        return boardPlayerVirtual.GetTotalScore(player1SubBoard) - boardPlayerVirtual.GetTotalScore(player2SubBoard);
    }

    public void AddCard(GameObject card) {
        if (actualPlayer == 1) {
            boardPlayerVirtual.AddCard(card, player1SubBoard, player2SubBoard);
        }

        else {
            boardPlayerVirtual.AddCard(card, player2SubBoard, player1SubBoard);
        }
    }

    public GameObject GetCardInRelativePosition(int pos) {
        bool[] hand;
        int count = 0;
        GameObject card = null;
        if (actualPlayer == 1) {
            hand = handCardsAvailableP1;
        }
        else {
            hand = handCardsAvailableP2;
        }
    
        for (int i = 0; i < hand.Length; i++) {
            //Debug.Log(actualPlayer + " Hand--> " +  hand[i] + "  count --> " + count + "  pos -->" + pos);
            
            //Debug.Log(count + " == " + pos);
            if (count == pos) {
                //Debug.Log("CardFinding");
                if (actualPlayer == 1) {
                    card =  currentAIHand[i];
                }
                else {
                    card = currentHumanHand[i];
                }
               

            }
            if (hand[i] == false) {
                ++count;
            }
        }

        //Debug.Log(card);
        return card;
    }
    // Creacion del tablero que le pasas al Minmax
    public Virtual_Board()
    {
        //Debug.Log("AI --> " + currentTurnAIBoard.Length + "  Human -->  " + currentTurnPlayerBoard.Length);

        player1SubBoard = new Field[currentTurnAIBoard.Length];
        player2SubBoard = new Field[currentTurnPlayerBoard.Length];
        currentTurnAIBoard.CopyTo(player1SubBoard, 0);
        currentTurnPlayerBoard.CopyTo(player2SubBoard, 0);

        handCardsAvailableP1 = new bool[currentAIHand.Count];
        handCardsAvailableP2 = new bool[currentHumanHand.Count];

        // La IA es la que empieza a usar minimax
        actualPlayer = 1;

        // Todas las cartas de primeras estan sin usar
        for (int i = 0; i < currentAIHand.Count; i++)
        {
            handCardsAvailableP1[i] = false;
        }
        for (int i = 0; i < currentHumanHand.Count; i++)
        {
            handCardsAvailableP2[i] = false;
        }
    }

    // 
    public Virtual_Board(PJBoard_Manager manager, int fieldsSize)
    {
        currentTurnPlayerBoard = new Field[fieldsSize];
        currentTurnAIBoard = new Field[fieldsSize];
        currentHumanHand = new List<GameObject>();
        currentAIHand = new List<GameObject>();
          
        boardPlayerVirtual = new PJBoard_Manager();

    }

    // Creacion por copia de trableros generados por MinMax, obteniendo las cartas usadas en el turno anterior, y el cambio de jugador.
    public Virtual_Board(Virtual_Board board)
    {
        //Debug.Log("CopyBoard");
        player1SubBoard = new Field[board.player1SubBoard.Length];
        player2SubBoard = new Field[board.player2SubBoard.Length];
        board.player1SubBoard.CopyTo(player1SubBoard,0);
        board.player2SubBoard.CopyTo(player2SubBoard, 0);

        handCardsAvailableP1 = board.handCardsAvailableP1;
        handCardsAvailableP2 = board.handCardsAvailableP2;

    }

    public void ChangePlayer() {
        if (actualPlayer == 2) {
            actualPlayer = 1;
        }
        else {
            actualPlayer = 2;
        }
    }

    public Field[] GetActualPlayer()
    {
        if (actualPlayer == 1)
        {
            return player1SubBoard;
        }
        else
        {
            return player2SubBoard;
        }
    }    

    public int GetActualPlayerHandCardsCount() {
        bool[] hand;
        int count = 0;

        if (actualPlayer == 1) {
            hand = handCardsAvailableP1;
        }
        else {
            hand =  handCardsAvailableP2;
        }

        for (int i = 0; i < hand.Length; i++) {
            if (hand[i] == false) {
                ++count;
            }
        }

        return count;
    }

    //Devuelve una puntuacion basada en una nueva Jugada
    public int EvaluateBoard(GameObject card)
    {

        RefreshtBoardScore();
        return scorePlayer2 - scorePlayer1;
    }

    // Valora de toda la mano, cual es la carta que mas puntos incrementa en el tablero
    public GameObject GetBestCardToUse()
    {
        GameObject bestCard = null;
        RefreshtBoardScore();
        for (int i = 0; i < handCardsAvailableP1.Length; i++)
        {
            if (handCardsAvailableP1[i] == true)
            {
                //Ni puta idea de como acceder a la mano de manera sencilla, pero le debes pasar algo asi.
                //CardScoreIncrement(/*playerHand[i]*/);
            }

        }

        return bestCard;
    }

    public void SetSubBoard(Field[] toCopy, bool isHuman) {
        if (isHuman) {
            toCopy.CopyTo(currentTurnPlayerBoard, 0);
        }
        else {
            toCopy.CopyTo(currentTurnAIBoard, 0);
        }
    }

    public void SetHand(Transform parent, bool isHuman)
    {
        int length = parent.transform.childCount; 
        if (isHuman)
        {
            currentHumanHand.Clear();

            for (int i = 0; i < length; i++)
            {
                currentHumanHand.Add(parent.GetChild(i).gameObject);
            }

        }
        else
        {
            currentAIHand.Clear();

            for (int i = 0; i < length; i++)
            {
                currentAIHand.Add(parent.GetChild(i).gameObject);
            }

        }
    }

    // Incremento de puntos en el tablero y cuando se añada la carta. 
    //Cuanto mas alto sea el incremento, mejore es la jugada para la IA
    public int CardScoreIncrement(GameObject card)
    {
        int scoreP1 = 0;
        int scoreP2 = 0;
        if (card.GetComponent<Card>().GetEffect() == ScoreEffects.effect.no)
        { // --> Si es una carta normal, suma su valor en funcion del efecto de la linea
            switch (card.GetComponent<Card>().GetField())
            {
                case FieldEnum.fields.CloseCombat:
                    if (GetActualPlayer()[0].GetAtributes().condition == ScoreEffects.effect.no)
                    {
                        scoreP1 = card.GetComponent<Card>().GetValue();
                    }
                    if (GetActualPlayer()[0].GetAtributes().condition == ScoreEffects.effect.x2)
                    {
                        scoreP1 = card.GetComponent<Card>().GetValue() * 2;
                    }
                    break;

            }
        }
        else
        { // --> En caso de que sea una carta de efecto
            switch (card.GetComponent<Card>().GetEffect())
            {
                case ScoreEffects.effect.sun:
                    if (GetActualPlayer()[0].GetAtributes().condition == ScoreEffects.effect.no)
                    {
                        scoreP1 = card.GetComponent<Card>().GetValue();
                    }
                    if (GetActualPlayer()[0].GetAtributes().condition == ScoreEffects.effect.x2)
                    {
                        scoreP1 = card.GetComponent<Card>().GetValue() * 2;
                    }
                    break;

            }

        }
        //card.player.hande
        return (scoreP1 - scoreP2) - (scorePlayer1 - scorePlayer2);
    }

    public void RefreshtBoardScore()
    {
        scorePlayer1 = player1SubBoard[0].GetAtributes().score +
                       player1SubBoard[1].GetAtributes().score +
                       player1SubBoard[2].GetAtributes().score;
        scorePlayer2 = player2SubBoard[0].GetAtributes().score +
                       player2SubBoard[1].GetAtributes().score +
                       player2SubBoard[2].GetAtributes().score;
    }
    public void AddCard(Card card)
    {


    }
    //// Si la carta es normal
    //public int PutNoEffectCard(Card card) {

    //    //si el campo del player no tiene efectos;
    //    switch (player1[(int)card.GetField()].GetAtributes()._effect) {
    //        case ScoreEffects.effect.no:
    //            return 100;
    //        case ScoreEffects.effect.x2:
    //            return Card


    //    }


    //}

    public bool HandsIsEmpty()
    {
        bool handIsEmpty = true;
        foreach (bool Card in handCardsAvailableP1)
        {
            if (Card == false)
            {
                handIsEmpty = false;
            }
        }
        foreach (bool Card in handCardsAvailableP2)
        {
            if (Card == false)
            {
                handIsEmpty = false;
            }
        }
        return handIsEmpty;
    }

    public bool ISGameEnd()
    {
        bool isEnd = false;
        if (HandsIsEmpty())
        {
            isEnd = true;
        }
        return isEnd;
    }


    //public int[] GetVirtualBoardModificated(GameObject card, bool isIA)
    //{
    //    virtualBoardFields = currentBoardFields;
    //    Field[] tempSubBoardIA = new Field[] { virtualBoardFields[0], virtualBoardFields[1], virtualBoardFields[2] };
    //    Field[] tempSubBoardPlayer = new Field[] { virtualBoardFields[3], virtualBoardFields[4], virtualBoardFields[5] };

    //    if (isIA)
    //    {
    //        managerAI.AddCard(card, tempSubBoardIA);
    //    }
    //    else
    //    {
    //        managerAI.AddCard(card, tempSubBoardPlayer);
    //    }

    //    int[] playersScore = new int[] { managerAI.GetTotalScore(tempSubBoardIA), managerHuman.GetTotalScore(tempSubBoardPlayer) };

    //    return playersScore;
    //}



    ///**
    // * Updates the information 
    // */
    //public void UnifySubBoards() //HandCarsAvailablePlayer2 is the AI 
    //{
    //    Field[] subBoardTemp = managerHuman.GetAllBoardPlayerInfo();
    //    currentBoardFields[3] = subBoardTemp[0];
    //    currentBoardFields[4] = subBoardTemp[1];
    //    currentBoardFields[5] = subBoardTemp[2];

    //    subBoardTemp = managerAI.GetAllBoardPlayerInfo();
    //    currentBoardFields[0] = subBoardTemp[0];
    //    currentBoardFields[1] = subBoardTemp[1];
    //    currentBoardFields[2] = subBoardTemp[2];
    //}

    //public Field[] UnifyFields(Field[] subBoard1, Field[] subBoard2) //HandCarsAvailablePlayer2 is the AI 
    //{
    //    Field[] subBoardTemp = currentBoardFields;

    //    subBoardTemp[0] = subBoard1[0];
    //    subBoardTemp[1] = subBoard1[1];
    //    subBoardTemp[2] = subBoard1[2];
    //    subBoardTemp[3] = subBoard2[3];
    //    subBoardTemp[4] = subBoard2[4];
    //    subBoardTemp[5] = subBoard2[5];

    //    return subBoardTemp;
    //}
}
