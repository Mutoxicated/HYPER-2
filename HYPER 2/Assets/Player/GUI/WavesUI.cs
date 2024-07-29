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
            waves.text = String.Format("{1} : {0}", Save.rData.Waves, Mathf.Abs(7-Mathf.CeilToInt(waitInterval.time)));
        }else {
            waves.text = String.Format("{0}", Save.rData.Waves);
        }
    }
}