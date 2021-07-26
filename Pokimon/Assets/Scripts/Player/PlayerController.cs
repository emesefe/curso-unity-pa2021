using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    public float speed;
    public LayerMask solidObjectsLayer, pokemonLayer;
    public float collisionDistance, battleDistance;

    public event Action OnPokemonEncountered;
    
    private bool isMoving;
    private Vector2 input;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void LateUpdate()
    {
        _animator.SetBool("Is Moving", isMoving);
    }
    
    public void HandleUpdate()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0)
            {
                input.y = 0;
            }

            if (input != Vector2.zero)
            {
                _animator.SetFloat("Move X", input.x);
                _animator.SetFloat("Move Y", input.y);
                
                Vector3 targetPosition = transform.position;
                targetPosition.x += input.x;
                targetPosition.y += input.y;

                if (IsAvailable(targetPosition))
                {
                    StartCoroutine(MoveTowards(targetPosition));
                }
            }
        }
    }

    private IEnumerator MoveTowards(Vector3 destination)
    {
        isMoving = true;
        
        while (Vector3.Distance(transform.position, destination) > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, 
                destination, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = destination;
        isMoving = false;

        CheckForPokemonBattle();
    }

    /// <summary>
    /// Comprueba que la zona a la que queremos acceder está disponible
    /// </summary>
    /// <param name = "target">Posición a la que queremos acceder</param>
    /// <returns>Devuelve true si el target está disponible y false en caso contrario</returns>
    private bool IsAvailable(Vector3 target)
    {
        if (Physics2D.OverlapCircle(target, collisionDistance, solidObjectsLayer) != null)
        {
            return false;
        }

        return true;
    }

    private void CheckForPokemonBattle()
    {
        if (Physics2D.OverlapCircle(transform.position, battleDistance, pokemonLayer) != null)
        {
            if (Random.Range(0, 100) < 10)
            {
                OnPokemonEncountered();
            }
        }
    }
}
