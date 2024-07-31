using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicScale : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 minScale;
    [SerializeField] private Vector3 maxScale;

    public void Update() {
        target.localScale = Vector3.Lerp(minScale, maxScale, MusicVisualization.T);
    }
}
