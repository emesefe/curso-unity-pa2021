using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    public float charactersPerSecond = 10f;
    
    [SerializeField] private Text dialogText;
    [SerializeField] private GameObject actionSelector;
    [SerializeField] private GameObject movementSelector;
    [SerializeField] private GameObject movementDescription;

    public IEnumerator SetDialog(string message)
    {
        // TODO: Reproducir sonido letras 
        dialogText.text = "";
        foreach (char character in message)
        {
            dialogText.text += character;
            yield return new WaitForSeconds(1 / charactersPerSecond);
        }
        // TODO: Mutear sonido letras
    }
}
