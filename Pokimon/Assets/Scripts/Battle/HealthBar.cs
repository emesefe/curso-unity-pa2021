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

    #endregion

    #region VARIABLES PRIVADAS

    private Image healthBarImage;

    #endregion

    /// <summary>
    /// Actualiza la barra de vida a partir del valor normlaizado de la misma
    /// </summary>
    /// <param name="normalizedValue">Valor de la vida normalizado entre 0 y 1</param>
    public void SetHP(float normalizedValue)
    {
        healthBar.transform.localScale = new Vector3(normalizedValue, 1f); 
        healthBar.GetComponent<Image>().color = ColorManager.SharedInstance.ColorRange(normalizedValue);
    }

    /// <summary>
    /// Actualiza la barra de vida a partir del valor normlaizado de la misma y muestra el cambio de forma suavizada
    /// Cambia de color cuando la barra baja de un cierto umbral
    /// </summary>
    /// <param name="normalizedValue">Valor de la vida normalizado entre 0 y 1</param>
    public IEnumerator SetSmoothHP(float normalizedValue)
    {
        /*float currentScale = healthBar.transform.localScale.x;
        float updateQuantity = currentScale - normalizedValue;

        while (currentScale - normalizedValue > Mathf.Epsilon)
        {
            currentScale -= updateQuantity * Time.deltaTime;
            healthBar.transform.localScale = new Vector3(currentScale, 1);
            

            yield return null;
        }*/

        Sequence sequence = DOTween.Sequence();
        sequence.Append(healthBar.transform.DOScaleX(normalizedValue, 1));
        sequence.Join(healthBar.GetComponent<Image>().DOColor(ColorManager.SharedInstance.ColorRange(normalizedValue), 1));
        yield return sequence.WaitForCompletion();
    }
}
