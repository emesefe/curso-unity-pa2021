using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    #region VARIABLES PUBLICAS
    
    public GameObject healthBar;

    public Color colorBar
    {
        get
        {
            float localScale = healthBar.transform.localScale.x;
            if (localScale < dangerHPThreshold)
            {
                return dangerHPColor;
                
            }else  if (localScale < lowHPThreshold)
            {
                return lowHPColor;
            }
            else
            {
                return highHPColor;
            }
        }
            
    }
    
    #endregion

    #region VARIABLES PRIVADAS

    private Image healthBarImage;

    [SerializeField] private Color highHPColor;
    [SerializeField] private Color lowHPColor;
    [SerializeField] private Color dangerHPColor;

    private float lowHPThreshold = 0.5f;
    private float dangerHPThreshold = 0.15f;
    
    #endregion


    /// <summary>
    /// Actualiza la barra de vida a partir del valor normlaizado de la misma
    /// </summary>
    /// <param name="normalizedValue">Valor de la vida normalizado entre 0 y 1</param>
    public void SetHP(float normalizedValue)
    {
        healthBar.transform.localScale = new Vector3(normalizedValue, 1f); 
        healthBar.GetComponent<Image>().color = colorBar;
    }

    /// <summary>
    /// Actualiza la barra de vida a partir del valor normlaizado de la misma y muestra el cambio de forma suavizada
    /// Cambia de color cuando la barra baja de un cierto umbral
    /// </summary>
    /// <param name="normalizedValue">Valor de la vida normalizado entre 0 y 1</param>
    public IEnumerator SetSmoothHP(float normalizedValue)
    {
        float currentScale = healthBar.transform.localScale.x;
        float updateQuantity = currentScale - normalizedValue;

        while (currentScale - normalizedValue > Mathf.Epsilon)
        {
            currentScale -= updateQuantity * Time.deltaTime;
            healthBar.transform.localScale = new Vector3(currentScale, 1);
            healthBar.GetComponent<Image>().color = colorBar;

            yield return null;
        }


        healthBar.transform.localScale = new Vector3(normalizedValue, 1);
    }
}
