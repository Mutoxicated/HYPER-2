using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScanlineSettingsGUI : MonoBehaviour {
    [SerializeField] private Toggle active;
    [SerializeField] private Slider lineSize;
    [SerializeField] private Slider modulo;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text lineSizeField;
    [SerializeField] private TMP_Text moduloField;

    public void Toggle(bool on) {
        lineSize.interactable = on; modulo.interactable = on;
        lineSizeField.color = on ? Color.white : Color.grey;
        moduloField.color = lineSizeField.color;
        title.color = lineSizeField.color;
    }

    public ScalineSettings Into() {
        return new ScalineSettings(active.isOn, lineSize.value, (int)lineSize.value);
    }

    public void Init() {
        active.onValueChanged.AddListener(Toggle);
        active.isOn = Save.pData.ScanlineSettings.active;
        lineSize.value = Save.pData.ScanlineSettings.lineSize;
        modulo.value = Save.pData.ScanlineSettings.modulo;
    }
}