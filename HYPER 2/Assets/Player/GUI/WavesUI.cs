using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WavesUI : MonoBehaviour
{
    [SerializeField] private TMP_Text waves;
    [SerializeField, Tooltip("From the Gameplay gameObject")] private Interval waitInterval;

    public void Update() {
        if (waitInterval.enabled == true) {
            waves.text = String.Format("Next Wave in {0}", Mathf.Abs(7-Mathf.CeilToInt(waitInterval.time)));
        }else {
            waves.text = String.Format("Wave {0}", Save.rData.Waves);
        }
    }
}