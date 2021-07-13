using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Life : MonoBehaviour
{
    [SerializeField]
    private float amount;

    public float Amount
    {
        get => amount;
        set
        {
            amount = value;
            
            if (amount <= 0)
            {
                Animator animator = GetComponent<Animator>();
                animator.SetTrigger("Play Die");
                
                Invoke("PlayExplosion", 1);

                Destroy(gameObject, 2);
            }
        }
    }

    void PlayExplosion()
    {
        ParticleSystem explosion = gameObject.GetComponentInChildren<ParticleSystem>();
        explosion.Play();
    }
}
