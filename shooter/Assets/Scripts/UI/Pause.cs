using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject pauseMenu;

    private void Awake()
    {
        pauseMenu.SetActive(false);
    }

    public void OpenPauseMenu()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }
    
    public void ClosePauseMenu()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }
}
