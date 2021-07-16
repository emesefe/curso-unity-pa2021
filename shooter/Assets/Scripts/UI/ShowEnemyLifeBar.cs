using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowEnemyLifeBar : MonoBehaviour
{
    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        canvas.worldCamera = Camera.main;
    }
}
