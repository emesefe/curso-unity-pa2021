using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [Tooltip("Tiempo después del cual se destruye el objeto")]
    public float destructionDelay;
    
    private void OnEnable()
    {
        Invoke("HideObject", destructionDelay);
    }

    private void HideObject()
    {
        gameObject.SetActive(false);
    }
}
