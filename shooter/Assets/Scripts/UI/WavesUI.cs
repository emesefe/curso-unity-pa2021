using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WavesUI : MonoBehaviour
{
    private TextMeshProUGUI _text;

    private void Start()
    {
        _text = gameObject.GetComponent<TextMeshProUGUI>();
        WaveManager.SharedInstance.OnWaveChanged.AddListener(RefreshText);
        RefreshText();
    }

    private void RefreshText()
    {
        _text.text = "waves: " + 
                     (WaveManager.SharedInstance.TotalWaves - WaveManager.SharedInstance.waveCount) + 
                     " / " + WaveManager.SharedInstance.TotalWaves;
    }
}
