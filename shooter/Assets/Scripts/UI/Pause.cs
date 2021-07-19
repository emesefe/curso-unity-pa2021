using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Pause : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject settingsMenu;

    public AudioMixerSnapshot gameSnapshot;
    public AudioMixerSnapshot pauseSnapshot;

    private void Awake()
    {
        pauseMenu.SetActive(false);
    }

    public void OpenPauseMenu()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        pauseSnapshot.TransitionTo(0.1f);
    }
    
    public void ClosePauseMenu()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        gameSnapshot.TransitionTo(0.1f);
    }
    
    public void OpenSettingsMenu()
    {
        settingsMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }
    
    public void CloseSettingsMenu()
    {
        settingsMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }
    
    
}
