using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DataPersistence : MonoBehaviour
{
    public TextMeshProUGUI actualScore, highScore, actualTime, bestTime;
    // public TextMeshProUGUI playerName, highScorePlayerName, bestTimePlayerName;


    private void Start()
    {
        // playerName.text = PlayerPrefs.GetString("Player Name");
        actualScore.text = string.Format("Score: {0}", PlayerPrefs.GetInt("Last Score"));
        actualTime.text = string.Format("Time: {0}", PlayerPrefs.GetFloat("Last Time"));

        // highScorePlayerName.text = PlayerPrefs.GetString("High Score Player Name");
        highScore.text = string.Format("{0}", PlayerPrefs.GetInt("High Score"));

        // bestTimePlayerName.text = PlayerPrefs.GetString("Best Time Player Name");
        bestTime.text = string.Format("{0}", PlayerPrefs.GetFloat("Best Time"));
    }
}
