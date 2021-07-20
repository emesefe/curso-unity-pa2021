using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameModeWaves : MonoBehaviour
{
    public GameObject winPanel;
    public GameObject gameOverPanel;
    
    

    public string actualName;

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
        RegisterScore();
        RegisterTime();
        gameOverPanel.SetActive(true);
    }
    
    void CheckWinCondition()
    {
        // Win
        if (EnemyManager.SharedInstance.enemyCount <= 0 &&
            WaveManager.SharedInstance.waveCount <= 0)
        {
            RegisterScore();
            RegisterTime();
            winPanel.SetActive(true);
        }
    }
    
    private void RegisterScore()
    {
        int actualScore = ScoreManager.SharedInstance.Amount;
        PlayerPrefs.SetInt("Last Score", actualScore);

        int highScore = PlayerPrefs.GetInt("High Score", 0);

        if (actualScore > highScore)
        {
            PlayerPrefs.SetInt("High Score", actualScore);
            // PlayerPrefs.SetString("High Score Player Name", actualName);
        }
    }
    
    private void RegisterTime()
    {
        float actualTime = Time.time;
        PlayerPrefs.SetFloat("Last Time", actualTime);

        float bestTime = PlayerPrefs.GetFloat("Best Time", 99999999.0f);

        if (actualTime < bestTime)
        {
            PlayerPrefs.SetFloat("Best Time", actualTime);
            // PlayerPrefs.SetString("Best Time Player Name", actualName);
        }
    }
}
