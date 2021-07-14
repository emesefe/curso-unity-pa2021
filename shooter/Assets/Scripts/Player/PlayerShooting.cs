using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Tooltip("Offset de la posición de instanciación de la bala respecto al player")]
    public Vector3 offset;
    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject bullet = ObjectPool.SharedInstance.GetFirstPooleableObject();
            bullet.layer = LayerMask.NameToLayer("Bullet Player");
            bullet.transform.position = transform.position + offset;
            bullet.transform.rotation = transform.rotation;
            bullet.SetActive(true);
        }
    }

}
