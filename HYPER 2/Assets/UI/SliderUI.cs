using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderUI : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text min;
    [SerializeField] private TMP_Text max;

    private void Start() {
        min.text = String.Format("{0}", slider.minValue);
        max.text = String.Format("{0}", slider.maxValue);
    }

    [ContextMenu("Init")]
    private void Init() {
        Start();
    }
}
