using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BattleDialogBox : MonoBehaviour
{
    public bool isWriting;
    public AudioClip[] characterClips;
    
    [SerializeField] private float charactersPerSecond = 10f;
    
    [SerializeField] private Text dialogText;
    [SerializeField] private GameObject actionSelector;
    [SerializeField] private GameObject movementSelector;
    [SerializeField] private GameObject movementDescription;

    [SerializeField] private List<Text> actionTexts;
    [SerializeField] private List<Text> movementTexts;
    [SerializeField] private Text ppText;
    [SerializeField] private Text typeText;
    [SerializeField] private Image typeBackgroundImage;
    
    private float timeToWaitAfterText = 1;

    public IEnumerator SetDialog(string message)
    {
        isWriting = true;
        dialogText.text = "";
        
        foreach (char character in message)
        {
            dialogText.text += character;
            if (character != ' ')
            {
                AudioManager.SharedInstance.RandomSoundEffect(characterClips);
            }
            yield return new WaitForSeconds(1 / charactersPerSecond);
        }
        
        yield return new WaitForSeconds(timeToWaitAfterText);
        isWriting = false;
    }

    #region Toggles de Textos

    public void ToggleDialogText(bool activate)
    {
        dialogText.enabled = activate;
    }

    public void ToggleActions(bool activate)
    {
        actionSelector.SetActive(activate);
    }

    public void ToggleMovements(bool activate)
    {
        movementSelector.SetActive(activate);
        movementDescription.SetActive(activate);
    }
    
    #endregion

    public void SelectAction(int selectedAction)
    {
        for (int i = 0; i < actionTexts.Count; i++)
        {
            actionTexts[i].color = (i == selectedAction ? ColorManager.SharedInstance.selectedColor : ColorManager.SharedInstance.defaultColor);
        }
    }
    
    public void SelectMovement(int selectedMovement, Move move)
    {
        for (int i = 0; i < movementTexts.Count; i++)
        {
            movementTexts[i].color = (i == selectedMovement ? ColorManager.SharedInstance.selectedColor : ColorManager.SharedInstance.defaultColor);
        }

        ppText.text = $"PP {move.PP} / {move.Base.PP}";
        typeText.text = move.Base.Type.ToString().ToUpper();

        ppText.color = ColorManager.SharedInstance.ColorRangePP((float) move.PP / move.Base.PP);
        typeBackgroundImage.color = ColorManager.ColorType.GetColorFromType(move.Base.Type);
    }

    public void SetPokemonMovements(List<Move> moves)
    {
        for (int i = 0; i < movementTexts.Count; i++)
        {
            movementTexts[i].text = (i < moves.Count ? moves[i].Base.Name : "---");
        }
    }
}
