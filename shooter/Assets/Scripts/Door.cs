using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Tooltip("Distancia a partir de la cual detecta al Player o enemigo")]
    public float detectionDistance;
    public LayerMask targetLayers;
    
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.CompareTag("Player") || otherCollider.CompareTag("Enemy"))
        {
            _animator.SetBool("character_nearby", true);
        }
    }

    private void OnTriggerExit(Collider otherCollider)
    {
        if (otherCollider.CompareTag("Player") || otherCollider.CompareTag("Enemy"))
        {
            _animator.SetBool("character_nearby", false);
        }
    }
}
