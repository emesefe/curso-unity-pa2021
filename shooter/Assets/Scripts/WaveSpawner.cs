using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Tooltip("Prefab del enemigo a spawnear")]
    public GameObject prefab;
    [Tooltip("Tiempo de inicio y fin de la oleada")]
    public float startTime, endTime;
    [Tooltip("Tiempo entre el spawning de enemigos")]
    public float spawnRate;
   
    private void Start()
    {
        WaveManager.SharedInstance.waves.Add(this);
        InvokeRepeating("SpawnEnemy", startTime, spawnRate);
        Invoke("EndWave", endTime);
    }
    

    private void SpawnEnemy()
    {
        Instantiate(prefab, transform.position, transform.rotation);
    }

    private void EndWave()
    {
        WaveManager.SharedInstance.waves.Remove(this);
        CancelInvoke();
    }
}
