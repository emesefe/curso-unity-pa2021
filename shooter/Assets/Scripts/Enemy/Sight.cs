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

    public Collider detectedTarget;

    private Collider[] colliders;

    // Hemos pasado el filtro de la distancia
    private void Update()
    {
        /*if (Physics.OverlapSphereNonAlloc(transform.position, sightDistance, this.colliders, targetLayers) == 0)
        {
            return;
        }*/
        
        colliders = Physics.OverlapSphere(transform.position, sightDistance, targetLayers);

        detectedTarget = null;
        
        foreach (Collider collider in colliders)
        {
            // Desde el centro del enemigo al centro del collider que ha entrado en el OverlapSphere
            Vector3 directionToCollider = Vector3.Normalize(collider.bounds.center - transform.position);

            // Ángulo entre vector forward de enemigo y collider del target
            float angleToCollider = Vector3.Angle(transform.forward, directionToCollider);

            // Si el ángulo cae dentro del campo de visión
            if (angleToCollider < angle)
            {
                // Comprobamos que en la línea de visión enemigo -> objetivo no haya obstáculos
                if (!Physics.Linecast(transform.position, collider.bounds.center, 
                    out RaycastHit hit, obstacleLayers))
                {
                    // Solo se ejecutará en el editor de Unity porque es un Gizmo
                    Debug.DrawLine(transform.position, collider.bounds.center, Color.green);
                    
                    // Guardamos la referencia del primer objetivo detectado
                    detectedTarget = collider;
                    break;
                }else
                {
                    Debug.DrawLine(transform.position, hit.point, Color.magenta);
                }
            }
        }
    }

    // Solo se ejecuta en el editor de Unity
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightDistance);
        
        Vector3 rightDir = Quaternion.Euler(0, angle, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, rightDir * sightDistance);
        Vector3 leftDir = Quaternion.Euler(0, -angle, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, leftDir * sightDistance);
    }
}
