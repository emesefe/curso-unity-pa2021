using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreUI : MonoBehaviour
{
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = gameObject.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        _text.text = "score: " + ScoreManager.SharedInstance.Amount;
    }
}