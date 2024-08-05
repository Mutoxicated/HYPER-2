using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VignetteSettingsGUI : MonoBehaviour {
    [SerializeField] private Toggle active;
    [SerializeField] private Slider intensity;
    [SerializeField] private Slider smoothness;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text intensityField;
    [SerializeField] private TMP_Text smoothnessField;

    public void Toggle(bool on) {
        intensity.interactable = on; smoothness.interactable = on;
        intensityField.color = on ? Color.white : Color.grey;
        smoothnessField.color = intensityField.color;
        title.color = intensityField.color;
    }

    public VignetteSettings Into() {
        return new VignetteSettings(active.isOn, intensity.value, smoothness.value);
    }

    public void Init() {
        active.onValueChanged.AddListener(Toggle);
        active.isOn = Save.pData.VignetteSettings.active;
        intensity.value = Save.pData.VignetteSettings.intensity;
        smoothness.value = Save.pData.VignetteSettings.smoothness;
    }
}
