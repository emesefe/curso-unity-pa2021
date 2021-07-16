using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sight : MonoBehaviour
{
    [Tooltip("Distancia a partir de la cual deja de ver el enemigo")]
    public float sightDistance;
    [Tooltip("Cono de visión")]
    public float angle;

    public LayerMask targetLayers;
    public LayerMask obstacleLayers;

    private void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, sightDistance, targetLayers);
        
        foreach (Collider collider in colliders)
        {
            
        }
    }
}
