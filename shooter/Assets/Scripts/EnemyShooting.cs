using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public GameObject prefab;

    public GameObject shootingPoint;

    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject bullet = Instantiate(prefab);
            bullet.transform.position = shootingPoint.transform.position;
            bullet.transform.rotation = shootingPoint.transform.rotation;
            
            Destroy(bullet, 2); //TODO: Cambiar la forma de destruir balas
        }*/
        
    }
}
