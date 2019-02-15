using UnityEngine;

public class AI_Player : Player{

    //[SerializeField] GameObject opponentHand;
    private Virtual_Board board;

    bool isEnter = false;

    private int MAX_DEPTH = 2;

    public GameObject bestCard;

    int Negamax(Virtual_Board _board, int depth, int alpha, int beta)
    {
        // Mejor puntuacion de tablero  
        int bestScore = 0;
        int score = 0;
        // Si llega al final de las ramas, devuelve la score del tablero.
        if (depth > MAX_DEPTH) {
            bestScore = _board.GetScore();
        }
        // Si esta en zonas intermedias
        else {
            //Creamos un MiniMax para todas las cartas que tenemos en la mano.
            for (int i = 0; i < _board.GetActualPlayerHandCardsCount(); i++) {
                // Duplicamoos el tablero para realizar el movimiento de cada carta.
                //Debug.Log(i);
                Virtual_Board newBoard = new Virtual_Board(_board);       
                newBoard.AddCard(newBoard.GetCardInRelativePosition(i));
                newBoard.ChangePlayer();
                // Y tomamos el mejor tablero de todos, anotando la mejor carta 
                // (de la profundidad superior) de la mano a la que corresponde.
                score = Negamax(newBoard, depth + 1, alpha, beta);
                Debug.Log("MiniMaxScore -->  " + score);
                if (score > bestScore) {
                    bestScore = score;
                    if (_board.GetCardInRelativePosition(i) != null) {
                        bestCard = _board.GetCardInRelativePosition(i);
                    }    
                }
            }
        }

        if (bestCard == null) {
            Debug.Log("NO SE ENCONTRO UNA CARTA MEJOR QUE EL TABLERO ACTUAL");            
        }

        return bestScore;
    }  

    protected override GameObject PickCard()
    {
        /* // Codigo sin terminar 
        //board = new Virtual_Board();
        //GameObject _bestCard = bestCard;
        //Debug.Log("IAENTER----------------------------- ¿?¿?" + bestCard);
        //Negamax(new Virtual_Board(board), 0, -1, 1);
        
        //isEnter = true;
        //Debug.Log("IAENTER----------------------------- " + "old" + bestCard +" new " + _bestCard);

        //Si no encuentra ninguna que mejore la puntuacion actual, saca la primera que este en su mano.

         */
        bestCard = hand.transform.GetChild(0).gameObject;
        
        return bestCard;
    }

    GameObject[] GetHand(GameObject handParent)
    {
        int nCards = handParent.transform.childCount;
        GameObject[] tempCardArray = new GameObject[nCards + 1]; // +1 because the surrender it is represented with an empty GameObject in the array. 

        for (int i = 0; i < nCards; i++)
        {
            tempCardArray[i] = handParent.transform.GetChild(i).gameObject;
        }

        return tempCardArray;
    }
}
