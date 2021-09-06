using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    #region VARIABLES PUBLICAS
    
    public GameObject healthBar;
    public Text currentHPText;
    public Text maxHPText;

    #endregion

    #region VARIABLES PRIVADAS

    private Image healthBarImage;
    private float healthBarAnimationDuration = 1f;

    #endregion

    /// <summary>
    /// Actualiza la barra de vida a partir del valor normlaizado de la misma
    /// </summary>
    /// <param name="normalizedValue">Valor de la vida normalizado entre 0 y 1</param>
    public void SetHP(Pokemon pokemon)
    {
        float normalizedValue = (float) pokemon.HP / pokemon.MaxHP;
        healthBar.transform.localScale = new Vector3(normalizedValue, 1f); 
        healthBar.GetComponent<Image>().color = ColorManager.SharedInstance.ColorRange(normalizedValue);
        currentHPText.text = $"{pokemon.HP}";
        maxHPText.text = $"/{pokemon.MaxHP}";
    }
    

    /// <summary>
    /// Actualiza la barra de vida a partir del valor normalizado de la misma y muestra el cambio de forma suavizada
    /// Cambia de color cuando la barra baja de un cierto umbral
    /// </summary>
    /// <param name="normalizedValue">Valor de la vida normalizado entre 0 y 1</param>
    public IEnumerator SetSmoothHP(Pokemon pokemon)
    {
        float normalizedValue = (float) pokemon.HP / pokemon.MaxHP;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(healthBar.transform.DOScaleX(normalizedValue, healthBarAnimationDuration));
        sequence.Join(healthBar.GetComponent<Image>().DOColor(ColorManager.SharedInstance.ColorRange(normalizedValue), 
            healthBarAnimationDuration));
        sequence.Join(currentHPText.DOCounter(pokemon.previousHPValue, pokemon.HP, 
            healthBarAnimationDuration));
        yield return sequence.WaitForCompletion();
    }
}
