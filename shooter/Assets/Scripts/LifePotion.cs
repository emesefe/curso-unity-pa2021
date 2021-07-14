using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifePotion : MonoBehaviour
{
    [Tooltip("Puntos de vida a recuperar con la poción")]
    public int lifePoints;

    public GameObject potionExplosion;

    private void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.gameObject.CompareTag("Player"))
        {
            Life life = otherCollider.GetComponent<Life>();

            if (life != null)
            {
                life.Amount += lifePoints;
            }

            ParticleSystem potionExplosionPS = Instantiate(potionExplosion, transform.position, transform.rotation)
                .GetComponent<ParticleSystem>();
            potionExplosionPS.Play();
            
            Destroy(gameObject);
        }
    }
    
    
}
