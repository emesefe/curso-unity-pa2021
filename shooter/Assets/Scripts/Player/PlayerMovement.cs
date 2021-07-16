using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    [Tooltip("Velocidad de Movimiento del Personaje en m/s")]
    [Range(0, 2000)]
    public float speedMovement = 1000;
    [Tooltip("Velocidad de Rotación de la Cámara en grados/s")]
    [Range(0, 1000)]
    public float speedRotation = 300;
    
    private Rigidbody _rigidbody;
    private float horizontalInput, verticalInput;
    private Animator _animator;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // Movement
        float space = speedMovement * Time.deltaTime;
        Vector3 dir = new Vector3(0, 0, verticalInput);
        // transform.Translate(dir.normalized * space);
        _rigidbody.AddRelativeForce(dir.normalized * space);
        
        // Camera Rotation
        float angle = speedRotation * Time.deltaTime;
        // transform.Rotate(0, horizontalInput * angle, 0);
        _rigidbody.AddRelativeTorque(0, horizontalInput * angle, 0);

        _animator.SetFloat("Speed", _rigidbody.velocity.magnitude);
    }
}
