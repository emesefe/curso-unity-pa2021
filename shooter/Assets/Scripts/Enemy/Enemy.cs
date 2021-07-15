using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Tooltip("Cantidad de puntos que se obtienen al derrotar al enemigo")]
    public int pointsAmount;

    private void Awake()
    {
        Life life = GetComponent<Life>();
        life.OnDeath.AddListener(DeathEnemy);
    }

    private void Start()
    {
        EnemyManager.SharedInstance.AddEnemy(this);
    }

    private void DeathEnemy()
    {
        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("Play Die");
                
        Invoke("PlayExplosion", 0.5f);
        
        Destroy(gameObject, 1);
        
        EnemyManager.SharedInstance.RemoveEnemy(this);
        ScoreManager.SharedInstance.Amount += pointsAmount;
    }
    
    private void PlayExplosion()
    {
        ParticleSystem explosion = gameObject.GetComponentInChildren<ParticleSystem>();
        explosion.Play();
    }

    private void OnDestroy()
    {
        Life life = GetComponent<Life>();
        life.OnDeath.RemoveListener(DeathEnemy);
    }
}
