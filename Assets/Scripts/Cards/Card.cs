using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour {

    [SerializeField] int value = 0;
    [SerializeField] FieldEnum.fields field;
    [SerializeField] ScoreEffects.effect ambientalEffect;

    [System.NonSerialized] public Player player;

    /// <summary>
    /// Returns the value of this card. 
    /// </summary>
    /// <returns></returns>
    public int GetValue()
    {
        return value;
    }

    /// <summary>
    /// Returns the destination field of this card. If it has no destination returns undefined. 
    /// </summary>
    /// <returns></returns>
    public FieldEnum.fields GetField()
    {
        return field;
    }

    /// <summary>
    /// Sets the field to this card. Used for the Commander's Horn card.
    /// </summary>
    /// <param name="field"></param>
    public void SetField(FieldEnum.fields field )
    {
        this.field = field;
    }

    /// <summary>
    /// Returns the effect of this card.
    /// </summary>
    /// <returns></returns>
    public ScoreEffects.effect GetEffect()
    {
        return ambientalEffect;
    }

    /// <summary>
    /// Make interactable this card by the interactability condition. 
    /// </summary>
    /// <param name="interactability"></param>
    public void MakeInteractable(bool interactability)
    {
        gameObject.GetComponent<Button>().interactable = interactability;
    }

    /// <summary>
    /// Method that is triggered on button press.
    /// </summary>
    public void CardButton(){   
        player.SetSelectedCard(this);
    }

    //public void PlaySound() // Not working, Event trigger component working to make it work.
    //{
    //    if (gameObject.GetComponent<Button>().IsInteractable())
    //    {
    //        transform.GetComponent<AudioSource>().Play();
    //    }
    //}
}
