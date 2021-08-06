using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public static ColorManager SharedInstance;

    public Color selectedColor => new Color(0.282353f, 0.7215686f, 0.4078432f, 1);
    public Color defaultColor => new Color(0.1960784f, 0.1960784f, 0.1960784f, 1);
    
    public Color highHPColor => new Color(0.282353f, 0.7215686f, 0.4078432f, 1);
    public Color lowHPColor => new Color(0.8784314f, 0.7529413f, 0.7529413f, 1);
    public Color dangerHPColor => new Color(0.7411765f, 0.1607843f, 0.1254902f, 1);
    
    private float lowHPThreshold = 0.5f;
    private float dangerHPThreshold = 0.2f;

    private void Awake()
    {
        SharedInstance = this;
    }
    
    public Color ColorRange(float finalScale)
    {
        if (finalScale < dangerHPThreshold)
        {
            return dangerHPColor;
                
        }else  if (finalScale < lowHPThreshold)
        {
            return lowHPColor;
        }
        else
        {
            return highHPColor;
        }
    }
    
    public Color ColorRangePP(float finalScale)
    {
        if (finalScale < dangerHPThreshold)
        {
            return dangerHPColor;
                
        }else  if (finalScale < lowHPThreshold)
        {
            return lowHPColor;
        }
        else
        {
            return defaultColor;
        }
    }
}
