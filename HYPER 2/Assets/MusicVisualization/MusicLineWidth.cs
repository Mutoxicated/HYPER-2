using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicLineWidth : MonoBehaviour
{
    [SerializeField] private LineRenderer lr;
    [SerializeField] private Vector2 minMaxWidthMultiplier;

    private void Update() {
        lr.widthMultiplier = Mathf.Lerp(minMaxWidthMultiplier.x, minMaxWidthMultiplier.y, MusicVisualization.T);
    }
}
