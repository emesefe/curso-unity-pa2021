using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuManager : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject buttonsMainPanel;
    
    public void StartGame()
    {
        SceneManager.LoadScene("Level 1");
    }
    
    public void ExitGame()
    {
        #if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        #else
        Application.Quit();
        #endif
    }
    
    public void OpenSettingsPanel()
    {
        settingsPanel.SetActive(true);
        buttonsMainPanel.SetActive(false);
    }
    
    public void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
        buttonsMainPanel.SetActive(true);
    }
}
