using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class MusicVisualization
{
    private static float t = 0f;

    public static float T {
        get { return t; }
    }

    public static void SetT(float t) {
        MusicVisualization.t = t;
    }
}
