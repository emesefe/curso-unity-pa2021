using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Tooltip("Velocidad de Movimiento del Personaje en m/s")]
    [Range(0, 1000)]
    public float speedMovement = 1;
    [Tooltip("Velocidad de Rotación de la Cámara en grados/s")]
    [Range(0, 1000)]
    public float speedRotation = 1;
    
    private Rigidbody rigidbody;
    private float horizontalInput, verticalInput;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // Movement
        float space = speedMovement * Time.deltaTime;
        Vector3 dir = new Vector3(0, 0, verticalInput);
        // transform.Translate(dir.normalized * space);
        rigidbody.AddRelativeForce(dir.normalized * space);
        
        // Camera Rotation
        float angle = speedRotation * Time.deltaTime;
        // transform.Rotate(0, horizontalInput * angle, 0);
        rigidbody.AddRelativeTorque(0, horizontalInput * angle, 0);
    }
}
