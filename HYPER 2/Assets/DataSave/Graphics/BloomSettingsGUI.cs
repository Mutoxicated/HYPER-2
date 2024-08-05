using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BloomSettingsGUI : MonoBehaviour {
    [SerializeField] private Toggle active;
    [SerializeField] private Slider intensity;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text intensityField;

    public void Toggle(bool on) {
        intensity.interactable = on;
        intensityField.color = on ? Color.white : Color.grey;
        title.color = intensityField.color;
    }
    
    public BloomSettings Into() {
        return new BloomSettings(active.isOn, intensity.value);
    }

    public void Init() {
        active.onValueChanged.AddListener(Toggle);
        active.isOn = Save.pData.BloomSettings.active;
        intensity.value = Save.pData.BloomSettings.intensity;
    }
}