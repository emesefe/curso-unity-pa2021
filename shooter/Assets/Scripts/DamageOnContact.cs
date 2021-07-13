using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnContact : MonoBehaviour
{
    public float damage;
    
    private void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.gameObject.CompareTag("Enemy") || otherCollider.gameObject.CompareTag("Player"))
        {
            gameObject.SetActive(false);
            // Destroy(otherCollider.gameObject);
        }

        Life life = otherCollider.GetComponent<Life>();

        if (life != null)
        {
            life.Amount -= damage;
        }
    }
}
