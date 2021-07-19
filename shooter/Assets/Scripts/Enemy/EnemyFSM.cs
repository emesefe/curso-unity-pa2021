using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFSM : MonoBehaviour
{
    public enum EnemyState { GoToBase, AttackBase, ChasePlayer, AttackPlayer}
    public EnemyState currentState;
    
    public float baseAttackDistance;
    public float playerAttackDistance;
    
    public GameObject shootingPoint;

    private Sight _sight; 
    private Transform baseTransform;
    private float tolPlayerNotDetected = 1.1f;

    private NavMeshAgent agent;
    
    private float shootRate = 1;
    private float lastShootTime;
    private ParticleSystem shootingEffect;
    private Animator _animator;
    
    # region METHODS

    private void Awake()
    {
        _sight = GetComponent<Sight>();
        agent = GetComponentInParent<NavMeshAgent>();
        _animator = GetComponentInParent<Animator>();
        shootingEffect = shootingPoint.gameObject.GetComponentInChildren<ParticleSystem>();
    }

    private void Start()
    {
        baseTransform = GameObject.FindWithTag("Base").transform;
    }

    private void Update()
    {
        _animator.SetFloat("Speed", agent.velocity.magnitude);
        
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
        _animator.SetBool("Shoot Bullet", false);
        
        agent.isStopped = false;
        agent.SetDestination(baseTransform.position);
            
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
        agent.isStopped = true;
        
        LookAt(baseTransform.position);
        ShootTarget();
    }
    
    private void ChasePlayer()
    {
        _animator.SetBool("Shoot Bullet", false);
        
        if (_sight.detectedTarget == null)
        {
            // El player ha desaparecido del campo de visión
            currentState = EnemyState.GoToBase;
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(_sight.detectedTarget.transform.position);
        
        float distanceToPlayer = Vector3.Distance(transform.position, _sight.detectedTarget.transform.position);
        if (distanceToPlayer < playerAttackDistance)
        {
            // El enemigo está cerca del player y lo puede atacar
            currentState = EnemyState.AttackPlayer;
        }
        
    }
    
    private void AttackPlayer()
    {
        agent.isStopped = true;
        
        if (_sight.detectedTarget == null)
        {
            // El player se ha ocultado
            currentState = EnemyState.GoToBase;
            return;
        }
        
        LookAt(_sight.detectedTarget.transform.position);
        ShootTarget();

        float distanceToPlayer = Vector3.Distance(transform.position, _sight.detectedTarget.transform.position);
        if (distanceToPlayer > playerAttackDistance * tolPlayerNotDetected)
        {
            // El player se aleja del enemigo
            currentState = EnemyState.ChasePlayer;
        }
        
    }

    private void ShootTarget()
    {
        if (Time.timeScale > 0)
        {
            float timeSinceLastShot = Time.time - lastShootTime;

            if (timeSinceLastShot < shootRate)
            {
                return;
            }

            lastShootTime = Time.time;
            
            _animator.SetBool("Shoot Bullet", true);
            GameObject bullet = ObjectPool.SharedInstance.GetFirstPooleableObject();
            bullet.layer = LayerMask.NameToLayer("Bullet Enemy");
            bullet.transform.position = shootingPoint.transform.position;
            bullet.transform.rotation = shootingPoint.transform.rotation;
            bullet.SetActive(true);
        
            shootingEffect.Play();
        }
    }

    private void LookAt(Vector3 targetPosition)
    {
        Vector3 directionToLook = Vector3.Normalize(targetPosition - transform.position);
        directionToLook.y = 0;
        transform.parent.forward = directionToLook;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, playerAttackDistance);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, baseAttackDistance);
    }
    
    # endregion
}
