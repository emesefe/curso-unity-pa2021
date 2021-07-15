using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerBulletsUI : MonoBehaviour
{
    public PlayerShooting targetShooting;
    
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = gameObject.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        _text.text = "bullets: " + targetShooting.bulletsAmount;
    }
}
