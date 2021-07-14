using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{
    public static WaveManager SharedInstance;
    public UnityEvent OnWaveChanged;
    public int waveCount
    {
        get => waves.Count;
    }
    
    private List<WaveSpawner> waves;

    private void Awake()
    {
        if (SharedInstance == null)
        {
            SharedInstance = this;
            waves = new List<WaveSpawner>();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void AddWave(WaveSpawner wave)
    {
        waves.Add(wave);
        OnWaveChanged.Invoke();
    }

    public void RemoveWave(WaveSpawner wave)
    {
        waves.Remove(wave);
        OnWaveChanged.Invoke();
    }
}
