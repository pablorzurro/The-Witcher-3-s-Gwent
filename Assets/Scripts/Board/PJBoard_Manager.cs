using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PJBoard_Manager : MonoBehaviour {
   
    [SerializeField] Board_Manager boardInstance;

    public GameObject fieldsParent;

    Field[] fields;

    List<int> unaffectedRows;
    #region effect instances
    const ScoreEffects.effect noEffect = ScoreEffects.effect.no;
    const ScoreEffects.effect fog = ScoreEffects.effect.fog;
    const ScoreEffects.effect rain = ScoreEffects.effect.rain;
    const ScoreEffects.effect snow = ScoreEffects.effect.snow;
    const ScoreEffects.effect x2 = ScoreEffects.effect.x2;
    const ScoreEffects.effect sun = ScoreEffects.effect.sun;
    #endregion

    #region field instances
   
    FieldEnum.fields selectedRowCH = FieldEnum.fields.undefined; //Selected field in commander's horn selection.
    const FieldEnum.fields blade = FieldEnum.fields.CloseCombat;
    const FieldEnum.fields bow = FieldEnum.fields.RangedCombat;
    const FieldEnum.fields catapult = FieldEnum.fields.SiegeCombat;
    #endregion

    /// <summary>
    /// Creation of the fields of this player and initialization.
    /// </summary>
    void Awake()
    {

        unaffectedRows = new List<int>();
        fields = new Field[]
        {
            new Field(blade, 0, noEffect),
            new Field(bow, 0, noEffect),
            new Field(catapult, 0, noEffect),

        };
        UpdateScore();
    }
   
    /// <summary>
    /// Add a card to this player board.
    /// </summary>
    /// <param name="card"></param>
    public void AddCard(GameObject card)
    {
        switch (card.GetComponent<Card>().GetEffect())
        {
            case noEffect:
                card.transform.SetParent(fieldsParent.transform.GetChild((int)card.GetComponent<Card>().GetField()));
                fields[(int)card.GetComponent<Card>().GetField()].AddScore(card.GetComponent<Card>().GetValue());
                break;
            case rain:
            case fog:
            case snow:
            case sun:
                boardInstance.ApplyGlobalEffect(this, card.GetComponent<Card>().GetEffect(), card);
                ApplyEffect(card.GetComponent<Card>().GetEffect());
                break;
            case x2:
                ApplyEffect(card.GetComponent<Card>().GetField(), fields); // It's a commander's horn 
                Destroy(card);
                break;
        }
        UpdateScore();
        boardInstance.UpdateBoardAndHands();
    }

    /// <summary>
    /// Add a card to the input fields virtually.
    /// </summary>
    /// <param name="card"></param>
    /// <param name="inputFields"></param>
    /// <returns></returns>
    public void AddCard(GameObject card, Field[] inputFields, Field[] inputFields2)
    {
        switch (card.GetComponent<Card>().GetEffect())
        {
            case noEffect:
                inputFields[(int)card.GetComponent<Card>().GetField()].AddScore(card.GetComponent<Card>().GetValue());
                break;
            case rain:
            case fog:
            case snow:
            case sun:
                ApplyEffect(card.GetComponent<Card>().GetEffect(), inputFields);
                ApplyEffect(card.GetComponent<Card>().GetEffect(), inputFields2);
                break;
            case x2:
                Debug.Log("wtf");
                ApplyEffect(card.GetComponent<Card>().GetField(), inputFields); // It's a commander's horn 
                break;
        }
    }

    /// <summary>
    /// Returns each field to its default values (e.g used in round changes). 
    /// </summary>
    public void ClearBoard()
    {
        for (int i = 0; i < fields.Length; i++)
        {
            fields[i].DefaultAtributes();
            foreach (Transform child in fieldsParent.transform.GetChild(i))
            {
                Destroy(child.gameObject);
            }
        }
    }

    /// <summary>
    /// Returns a list of unaffected rows (fields) on the input fields.
    /// </summary>
    /// <param name="inputFields"></param>
    /// <returns></returns>
    public List<int> GetUnaffectedRows(Field[] inputFields)
    {
        unaffectedRows.Clear();

        foreach (Field item in inputFields)
        {
            if (item.GetAtributes().condition == noEffect)
            {
                unaffectedRows.Add((int)item.GetLine());
            }
        }
        return unaffectedRows;
    }

    /// <summary>
    /// Returns a list of unaffected rows (fields) on this player board.
    /// </summary>
    /// <returns></returns>
    public List<int> GetUnaffectedRows()
    {
        unaffectedRows.Clear();

        foreach (Field item in fields)
        {
            if (item.GetAtributes().condition == noEffect)  // Limitant factor 
            {
                unaffectedRows.Add((int)item.GetLine()); // The values stored in the list are the Field Enum values casted to int.
            }
        }
        return unaffectedRows;
    }

    /// <summary>
    /// Returns a copy of this player board fields.
    /// </summary>
    /// <returns></returns>
    public Field[] GetAllBoardPlayerInfo()
    {
        Field[] temp = new Field[fields.Length];

        fields.CopyTo(temp, 0);
        return temp;
    }

    ///// <summary>
    ///// Returns a copy of the input array of fields.
    ///// </summary>
    ///// <param name="inputFields"></param>
    ///// <returns></returns>
    //public Field[] CopySubBoard(Field[] inputFields)
    //{
    //    Field[] temp = new Field[inputFields.Length];
       
    //    return temp;
    //}

    /// <summary>
    /// Returns the selected row. Undefined until user input.
    /// 
    /// </summary>
    /// <returns></returns>
    public FieldEnum.fields GetSelectedRow() //Used for the commander's horn line selected
    {
        return selectedRowCH;
    }

    /// <summary>
    /// Casts the input int value to FieldEnum item and sets the selected row (field) to it. Method used by the buttons of the lines in scene.
    /// </summary>
    /// <param name="row"></param>
    public void SetSelectedRow(int row) // The method that the buttons call when are pressed, here it is selected the line to be boosted (x2)
    {
        selectedRowCH = (FieldEnum.fields)row;
    }

    #region Score Management
    [SerializeField] private GameObject scoreTextParent;
    int totalScore;

    /// <summary>
    /// Updates total score and texts.
    /// </summary>
    void UpdateScore()
    {
        UpdateTotalScore();
        UpdateText();
    }

    /// <summary>
    /// Update score texts.
    /// </summary>
    void UpdateText()
    {
        for (int i = 0; i < fields.Length; i++)
        {
            scoreTextParent.transform.GetChild(i).GetComponent<Text>().text = fields[i].GetAtributes().score.ToString();
        }
    }

    /// <summary>
    /// Updates this fields total score. 
    /// </summary>
    void UpdateTotalScore()
    {
        totalScore = 0;

        for (int i = 0; i < fields.Length; i++)
        {
            totalScore += fields[i].GetAtributes().score;
        }
    }

    /// <summary>
    /// Returns the summatory of the input fields scores.
    /// </summary>
    /// <param name="inputFields"></param>
    /// <returns></returns>
    public int GetTotalScore(Field[] inputFields)
    {
        int totalScoreTemp = 0;

        for (int i = 0; i < inputFields.Length; i++)
        {
            totalScore += inputFields[i].GetAtributes().score;
        }

        return totalScoreTemp;
    }


    /// <summary>
    /// Returns the summatory of all field scores (this player).
    /// </summary>
    /// <returns></returns>
    public int GetTotalScore()
    {
        return totalScore;
    }


    /// <summary>
    /// Returns the score from a row with a specific field enumeration item. 
    /// </summary>
    /// <param name="inputFields"></param>
    /// <returns></returns>
    public int[] GetRowsScore(Field[] inputFields)
    {
        int[] temp = new int[inputFields.Length];

        for (int i = 0; i < inputFields.Length; i++)
        {
            temp[i] = inputFields[i].GetAtributes().score;
        }

        return temp;
    }

    #endregion

    #region Effect Management
    public void ApplyEffect(ScoreEffects.effect effect)
    {
        switch (effect)
        {
            case rain:
                fields[(int)catapult].SetEffect(effect);
                break;
            case fog:
                fields[(int)bow].SetEffect(effect);
                break;
            case snow:
                fields[(int)blade].SetEffect(effect);
                break;
            case sun:
                DefaultRowEffects();
                break;
        }
        UpdateScore();
    }

    public Field[] ApplyEffect(ScoreEffects.effect effect, Field[] inputFields)
    {
        switch (effect)
        {
            case rain:
                inputFields[(int)catapult].SetEffect(effect);
                break;
            case fog:
                inputFields[(int)bow].SetEffect(effect);
                break;
            case snow:
                inputFields[(int)blade].SetEffect(effect);
                break;
            case sun:
                DefaultRowEffects(inputFields);
                break;
        }
        return inputFields;
    }

    void ApplyEffect(FieldEnum.fields field, Field[] inputFields)
    {
        inputFields[(int)field].SetEffect(x2);
    }

    void DefaultRowEffects(Field[] inputFields)
    {
        for (int i = 0; i < inputFields.Length; i++)
        {
            if (inputFields[i].GetAtributes().condition != noEffect && inputFields[i].GetAtributes().condition != x2)
            {
                inputFields[i].RevertScore();
            }
        }
    }

    void DefaultRowEffects()
    {
        for (int i = 0; i < fields.Length; i++)
        {
            if (fields[i].GetAtributes().condition != noEffect && fields[i].GetAtributes().condition != x2)
            {
                fields[i].RevertScore();
            }
        }
    }
    #endregion

}