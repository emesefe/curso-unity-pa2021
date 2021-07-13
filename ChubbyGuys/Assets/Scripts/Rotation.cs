using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    [Range(-180, 180)]
    public float speed;
    
    void Update()
    {
        if (this.gameObject.CompareTag("Axe"))
        {
            transform.Rotate(speed * Time.deltaTime, 0, 0);
        }
        
        if (this.gameObject.CompareTag("Spyked Cylinder"))
        {
            transform.Rotate(0, speed * Time.deltaTime,0);
        }

        if (this.gameObject.CompareTag("Spyked Cylinder Swing"))
        {
            float maxRotation = 90f;
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 
                maxRotation * Mathf.Sin(Time.time * speed));
            
            // transform.Rotate(0, 0,speed * Time.deltaTime);
        }
    }
}
