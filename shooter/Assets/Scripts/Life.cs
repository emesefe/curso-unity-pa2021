using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Life : MonoBehaviour
{ 
    public UnityEvent OnDeath;
    public float Amount
    {
        get => amount;
        set
        {
            amount = value;
            
            if (amount <= 0)
            {
                OnDeath.Invoke(); // Invocamos al Evento OnDeath
            }
        }
    }

    [SerializeField]
    private float amount;

   
}
