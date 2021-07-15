using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeWaves : MonoBehaviour
{
    public GameObject winPanel;
    public GameObject gameOverPanel;
    
    [SerializeField]
    private Life playerLife;
    [SerializeField]
    private Life baseLife;

    private void Start()
    {
        playerLife.OnDeath.AddListener(CheckGameOverCondition);
        baseLife.OnDeath.AddListener(CheckGameOverCondition);
        
        EnemyManager.SharedInstance.OnEnemyChanged.AddListener(CheckWinCondition);
        WaveManager.SharedInstance.OnWaveChanged.AddListener(CheckWinCondition);
    }

    void CheckGameOverCondition()
    {
        // Game Over
        gameOverPanel.SetActive(true);
    }
    
    void CheckWinCondition()
    {
        // Win
        if (EnemyManager.SharedInstance.enemyCount <= 0 &&
            WaveManager.SharedInstance.waveCount <= 0)
        {
            winPanel.SetActive(true);
        }
    }
}
