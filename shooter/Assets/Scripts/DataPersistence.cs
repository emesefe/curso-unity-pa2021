using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DataPersistence : MonoBehaviour
{
    public TextMeshProUGUI actualScore, highScore, actualTime, bestTime;
    public TextMeshProUGUI playerName, highScorePlayerName, bestTimePlayerName;

    public string actualName;

    private void Start()
    {
        playerName.text = PlayerPrefs.GetString("Player Name");
        actualScore.text = "Score: " + PlayerPrefs.GetInt("Last Score");
        actualTime.text = "Score: " + PlayerPrefs.GetFloat("Last Time");

        highScorePlayerName.text = PlayerPrefs.GetString("High Score Player Name");
        highScore.text = "High Score:" + PlayerPrefs.GetInt("High Score");
        
        bestTimePlayerName.text = PlayerPrefs.GetString("Best Time Player Name");
        highScore.text = "Best Time:" + PlayerPrefs.GetFloat("Best Time");
    }

    private void RegisterScore()
    {
        int actualScore = ScoreManager.SharedInstance.Amount;
        PlayerPrefs.SetInt("Last Score", actualScore);

        int highScore = PlayerPrefs.GetInt("High Score", 0);
        string highScorePlayerName = PlayerPrefs.GetString("High Score Player Name", "Name High Score");

        if (actualScore > highScore)
        {
            
            PlayerPrefs.SetInt("High Score", actualScore);
            PlayerPrefs.SetString("Best Time Player Name", actualName);
        }
    }
    
    private void RegisterTime()
    {
        float actualTime = Time.time;
        PlayerPrefs.SetFloat("Last Time", actualTime);

        float bestTime = PlayerPrefs.GetFloat("Best Time", 99999999.0f);
        string bestTimePlayerName = PlayerPrefs.GetString("Best Time Player Name", "Name Best Time");

        if (actualTime < bestTime)
        {
            PlayerPrefs.SetFloat("Best Time", bestTime);
            PlayerPrefs.SetString("Best Time Player Name", actualName);
        }
    }
}
