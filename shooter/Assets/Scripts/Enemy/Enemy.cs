using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Tooltip("Cantidad de puntos que se obtienen al derrotar al enemigo")]
    public int pointsAmount;

    public GameObject explosionEffect;

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
                
        Invoke("PlayExplosion", 1);
        Destroy(gameObject, 2);
        
        EnemyManager.SharedInstance.RemoveEnemy(this);
        ScoreManager.SharedInstance.Amount += pointsAmount;
    }
    
    private void PlayExplosion()
    {
        ParticleSystem explosion = explosionEffect.GetComponent<ParticleSystem>();
        explosion.Play();
    }

    private void OnDestroy()
    {
        Life life = GetComponent<Life>();
        life.OnDeath.RemoveListener(DeathEnemy);
    }
}
