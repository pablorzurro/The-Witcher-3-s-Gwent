using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Its a helper to both player board managers. Apply global climate effects to both boards. 
/// </summary>
public class Board_Manager : MonoBehaviour
{
    [SerializeField] PJBoard_Manager managerHuman;
    [SerializeField] PJBoard_Manager managerAI;

    Transform effectLayout;

    Virtual_Board virtualBoard;

    private void Awake()
    {
        effectLayout = transform.GetChild(transform.childCount - 1);

        virtualBoard = new Virtual_Board(managerHuman,3);
    }

    public void ApplyGlobalEffect(PJBoard_Manager sender, ScoreEffects.effect effect, GameObject card)
    {
        if (sender == managerAI)
        {
            managerHuman.ApplyEffect(effect);
        }
        else
        {
            managerAI.ApplyEffect(effect);
        }

        if (effect == ScoreEffects.effect.sun)
        {
            for (int i = 0; i < effectLayout.childCount; i++)
            {
                Destroy(effectLayout.GetChild(i).gameObject);
            }
            Destroy(card);
        }
        else
        {
            card.transform.SetParent(transform.GetChild(transform.childCount - 1));
        }
    }

    public void UpdateBoardAndHands()
    {
        UpdateSubBoardsBoard();
        UpdateVirtualBoardHands();
    }

    public void UpdateSubBoardsBoard()
    {
        Field[] temp = managerHuman.GetAllBoardPlayerInfo();
        virtualBoard.SetSubBoard(temp, true);

        temp = managerAI.GetAllBoardPlayerInfo();
        virtualBoard.SetSubBoard(temp, false);
    }

    public void UpdateVirtualBoardHands()
    {
        virtualBoard.SetHand(managerHuman.fieldsParent.transform.GetChild(3), true);
        virtualBoard.SetHand(managerAI.fieldsParent.transform.GetChild(3), false);
    }
}