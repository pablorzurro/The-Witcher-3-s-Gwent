using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field{
    /// <summary>
    /// Struct that stores the score and condition atributes of this field.
    /// </summary>
    /// <param name="sc"></param>
    /// <param name="eff"></param>
    public struct FieldAtributes
    {
        public int score;
        public int scoreBackup;

        public ScoreEffects.effect condition; //effect

        public FieldAtributes(int sc, ScoreEffects.effect eff)
        {
            score = sc;
            condition = eff;
            scoreBackup = -1;
        }
    }

    private FieldEnum.fields line;
    private FieldAtributes fieldAtributes;
    private int _nCards;

    /// <summary>
    /// Main constructor of this class.
    /// </summary>
    /// <param name="fName"></param>
    /// <param name="sc"></param>
    /// <param name="eff"></param>
    public Field(FieldEnum.fields fName, int sc, ScoreEffects.effect eff)
    {
        line = fName;
        fieldAtributes = new FieldAtributes(sc, eff);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="inputField"></param>
    public Field(Field inputField) // Copy constructor
    {
        line = inputField.line;
        fieldAtributes = inputField.fieldAtributes;
        _nCards = inputField._nCards;
    }

    /// <summary>
    /// Adds the card value to this field score.
    /// </summary>
    /// <param name="score"></param>
    public void AddScore(int score)
    {
        ++_nCards;

        if (isAffected())
        {
            AddScoreWhileAffected(score);
        }
        else
        {
            fieldAtributes.score += score;
        }
    }

    /// <summary>
    /// Sets the condition by the input effect.
    /// </summary>
    /// <param name="effect"></param>
    public void SetEffect(ScoreEffects.effect effect)
    {
        fieldAtributes.condition = effect;

        if(effect == ScoreEffects.effect.x2)
        {
            AffectScore();
        }
        else
        {
            AffectScore(_nCards);
        }
    }

    /// <summary>
    /// Affects the score by doubling its value.
    /// </summary>
    public void AffectScore()
    {
        fieldAtributes.scoreBackup = fieldAtributes.score;
        fieldAtributes.score *= 2;
    }

    /// <summary>
    /// Affects the score with the number of cards in this line. Used for climate effect.
    /// </summary>
    /// <param name="nCardsP"></param>
    public void AffectScore(int nCardsP)
    {
        fieldAtributes.scoreBackup = fieldAtributes.score;
        fieldAtributes.score = nCardsP;
    }

    /// <summary>
    /// Adds a score while in a effect condition
    /// </summary>
    /// <param name="score"></param>
    private void AddScoreWhileAffected(int score)
    {
        if (fieldAtributes.scoreBackup == -1)
        {
            fieldAtributes.scoreBackup = 0;
        }
        fieldAtributes.scoreBackup += score;

        if (fieldAtributes.condition == ScoreEffects.effect.fog || fieldAtributes.condition == ScoreEffects.effect.rain || fieldAtributes.condition == ScoreEffects.effect.snow)
        {
            ++fieldAtributes.score;
        }
        else
        {
            fieldAtributes.score = fieldAtributes.scoreBackup * 2;
        }
    }

    /// <summary>
    /// Reverts the values of score and its backup
    /// </summary>
    public void RevertScore()
    {
        fieldAtributes.score = fieldAtributes.scoreBackup;
        fieldAtributes.scoreBackup = -1;
    }

    /// <summary>
    /// Returns the line of this field.
    /// </summary>
    /// <returns></returns>
    public FieldEnum.fields GetLine()
    {
        return line;
    }


    /// <summary>
    /// Returns the atributes struct instance.
    /// </summary>
    /// <returns></returns>
    public FieldAtributes GetAtributes()
    {
        return fieldAtributes;
    }


    /// <summary>
    /// Returns the condition of this field. 
    /// </summary>
    /// <returns></returns>
    public bool isAffected()
    {
        return fieldAtributes.condition != ScoreEffects.effect.no;
    }

    /// <summary>
    /// Reset the atributes to them initial value.
    /// </summary>
    public void DefaultAtributes()
    {
        fieldAtributes.score = 0;
        _nCards = 0;

        if (isAffected())
        {
            fieldAtributes.scoreBackup = -1;
            fieldAtributes.condition = ScoreEffects.effect.no;
        }
    }
}