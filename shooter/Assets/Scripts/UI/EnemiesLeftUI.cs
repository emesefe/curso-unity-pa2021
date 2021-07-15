using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemiesLeftUI : MonoBehaviour
{
    private TextMeshProUGUI _text;

    private void Start()
    {
        _text = gameObject.GetComponent<TextMeshProUGUI>();
        EnemyManager.SharedInstance.OnEnemyChanged.AddListener(RefreshText);
        RefreshText();
    }

    private void RefreshText()
    {
        _text.text = "enemies left: " + EnemyManager.SharedInstance.enemyCount;
    }
}
