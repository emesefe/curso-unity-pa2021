using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool SharedInstance;
    public GameObject prefab;
    public List<GameObject> pooledObjets;
    public int amountToPool;

    private void Awake()
    {
        // Nos aseguramos de que no hay otra piscina ya existente
        if (SharedInstance == null)
        {
            SharedInstance = this;   
        }
        else
        {
            Destroy(this.gameObject);
        }
        
    }

    private void Start()
    {
        // Inicializamos la Piscina de Objetos
        pooledObjets = new List<GameObject>();
        GameObject tmp; // Objeto temporal en la Piscina de Objetos
        for (int i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(prefab);
            tmp.SetActive(false);
            pooledObjets.Add(tmp);
        }
    }
    
    // Busca el primer Game Object de la piscina que no está siendo usado
    public GameObject GetFirstPooleableObject()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!pooledObjets[i].activeInHierarchy)
            {
                return pooledObjets[i];
            }
        }

        return null;
    }
}
