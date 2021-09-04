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
    public Color lowHPColor => new Color(0.8784314f, 0.7529413f, 0.372549f, 1);
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
    
    public class ColorType
    {
        private static Color[] colorPokemonType =
        {
            Color.white, // None
            new Color(0.8193042f, 0.9333333f, 0.5254902f), // Bug
            new Color(0.735849f, 0.6178355f, 0.5588287f), // Dark
            new Color(0.6556701f, 0.5568628f, 0.7647059f), // Dragon
            new Color(0.9942768f, 1f, 0.5707547f), // Electric
            new Color(0.9339623f, 0.7621484f, 0.9339623f), // Fairy
            new Color(0.735849f, 0.5600574f, 0.5310609f), // Fight
            new Color(0.990566f, 0.5957404f, 0.5279903f), // Fire
            new Color(0.7358491f, 0.7708895f, 0.9811321f), // Flying
            new Color(0.6094251f, 0.6094251f, 0.7830189f), // Ghost
            new Color(0.4103774f, 1, 0.6846618f), // Grass
            new Color(0.9433962f, 0.7780005f, 0.5562478f), // Ground
            new Color(0.7216981f, 0.9072328f, 1), // Ice
            new Color(0.8734059f, 0.8773585f, 0.8235582f), // Normal
            new Color(0.6981132f, 0.4774831f, 0.6539872f), // Poison
            new Color(1, 0.6650944f, 0.7974522f), // Psychic
            new Color(0.8584906f, 0.8171859f, 0.6519669f), // Rock
            new Color(0.7889819f, 0.7889819f, 0.8490566f), // Steel
            new Color(0.5613208f, 0.7828107f, 1) // Water
        };

        public static Color GetColorFromType(PokemonType type)
        {
            return colorPokemonType[(int) type];
        }
    }
}
