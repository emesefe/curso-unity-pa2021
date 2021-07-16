using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFSM : MonoBehaviour
{
    public enum EnemyState { GoToBase, AttackBase, ChasePlayer, AttackPlayer}
    public EnemyState currentState;
    
    public float baseAttackDistance;
    public float playerAttackDistance;

    private Sight _sight; 
    private Transform baseTransform;
    private float tolPlayerNotDetected = 1.1f;

    private void Awake()
    {
        _sight = GetComponent<Sight>();
    }

    private void Start()
    {
        baseTransform = GameObject.FindWithTag("Base").transform;
    }

    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.GoToBase:
                GoToBase();
                break;
            
            case EnemyState.AttackBase:
                AttackBase();
                break;
            
            case EnemyState.ChasePlayer:
                ChasePlayer();
                break;
            
            case EnemyState.AttackPlayer:
                AttackPlayer();
                break;
        }
    }

    private void GoToBase()
    {
        if (_sight.detectedTarget != null)
        {
            // El player ha sido detetado
            currentState = EnemyState.ChasePlayer;
        }

        float distanceToBase = Vector3.Distance(transform.position, baseTransform.position);
        if (distanceToBase < baseAttackDistance)
        {
            // El enemigo está cerca de la base y la puede atacar
            currentState = EnemyState.AttackBase;
        }
    }
    
    private void AttackBase()
    {
        // Si el enemigo es capaz de llegar a la base, se queda atacando la base
    }
    
    private void ChasePlayer()
    {
        if (_sight.detectedTarget == null)
        {
            // El player ha desaparecido del campo de visión
            currentState = EnemyState.GoToBase;
            return;
        }
        
        float distanceToPlayer = Vector3.Distance(transform.position, _sight.detectedTarget.transform.position);
        if (distanceToPlayer < playerAttackDistance)
        {
            // El enemigo está cerca del player y lo puede atacar
            currentState = EnemyState.AttackPlayer;
        }
        
    }
    
    private void AttackPlayer()
    {
        if (_sight.detectedTarget == null)
        {
            // El player se ha ocultado
            currentState = EnemyState.GoToBase;
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, _sight.detectedTarget.transform.position);
        if (distanceToPlayer > playerAttackDistance * tolPlayerNotDetected)
        {
            // El player se aleja del enemigo
            currentState = EnemyState.ChasePlayer;
        }
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, playerAttackDistance);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, baseAttackDistance);
    }
}
