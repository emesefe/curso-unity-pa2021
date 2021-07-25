using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public GameObject healthBar;

    private Image healthBarImage;

    [SerializeField] private Color highHPColor;
    [SerializeField] private Color lowHPColor;
    [SerializeField] private Color dangerHPColor;

    [SerializeField] private float lowHPThreshold = 0.4f;
    [SerializeField] private float dangerHPThreshold = 0.1f;

    private void Start()
    {
        healthBarImage = healthBar.GetComponent<Image>();
    }


    /// <summary>
    /// Actualiza la barra de vida a partir del valor normlaizado de la misma
    /// </summary>
    /// <param name="normalizedValue">Valor de la vida normalizado entre 0 y 1</param>
    public void SetHP(float normalizedValue)
    {
        healthBar.transform.localScale = new Vector3(normalizedValue, 1f); 
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
            
            if (currentScale < dangerHPThreshold)
            {
                healthBarImage.color = dangerHPColor;
            }else  if (currentScale < lowHPThreshold)
            {
                healthBarImage.color = lowHPColor;
            }
            else
            {
                healthBar.GetComponent<Image>().color = highHPColor;
            }

            yield return null;
        }


        healthBar.transform.localScale = new Vector3(normalizedValue, 1);
    }
}
