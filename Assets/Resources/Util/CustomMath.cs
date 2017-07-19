using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CustomMath
{
    
    public static float Slerp(float a, float b, float t)
    {
        Mathf.Clamp01(t);

        float factor = 1.0f - (1 + Mathf.Cos(t * Mathf.PI)) / 2.0f; // [0,1]

        if (a < b)
        {
            return factor * (b - a) + a;
        }
        else
        {
            factor = 1.0f - factor; // [1,0]
            return factor * (a - b) + b;
        }
    }

}
