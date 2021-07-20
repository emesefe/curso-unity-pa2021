using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasLookAt : MonoBehaviour
{
    private Canvas _canvas;
    private GameObject targetToLookAt;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        targetToLookAt = GameObject.FindWithTag("MainCamera");
        _canvas.worldCamera = Camera.main;
    }

    private void Update()
    {
        transform.forward = targetToLookAt.transform.position - transform.position;
    }
}
