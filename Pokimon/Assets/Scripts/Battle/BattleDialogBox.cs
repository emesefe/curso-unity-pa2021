using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    public bool isWriting;
    
    [SerializeField] private float charactersPerSecond = 10f;
    
    [SerializeField] private Text dialogText;
    [SerializeField] private GameObject actionSelector;
    [SerializeField] private GameObject movementSelector;
    [SerializeField] private GameObject movementDescription;

    [SerializeField] private List<Text> actionTexts;
    [SerializeField] private List<Text> movementTexts;
    [SerializeField] private Text ppText;
    [SerializeField] private Text typeText;

    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color defaultColor = new Color(0.1960784f, 0.1960784f, 0.1960784f, 1);

    private float timeToWaitAfterText = 1;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public IEnumerator SetDialog(string message)
    {
        isWriting = true;
        dialogText.text = "";
        
        foreach (char character in message)
        {
            dialogText.text += character;
            if (character != ' ')
            {
                _audioSource.Play();
            }
            yield return new WaitForSeconds(1 / charactersPerSecond);
        }
        _audioSource.Stop();
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
            actionTexts[i].color = (i == selectedAction ? selectedColor : defaultColor);
        }
    }
    
    public void SelectMovement(int selectedMovement, Move move)
    {
        for (int i = 0; i < movementTexts.Count; i++)
        {
            movementTexts[i].color = (i == selectedMovement ? selectedColor : defaultColor);
        }

        ppText.text = $"PP {move.PP} / {move.Base.PP}";
        typeText.text = move.Base.Type.ToString().ToUpper();

        ppText.color = (move.PP <= 0 ? Color.red : defaultColor);
    }

    public void SetPokemonMovements(List<Move> moves)
    {
        for (int i = 0; i < movementTexts.Count; i++)
        {
            movementTexts[i].text = (i < moves.Count ? moves[i].Base.Name : "---");
        }
    }
}
