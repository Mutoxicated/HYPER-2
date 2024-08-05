using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChromaAbbSettingsGUI : MonoBehaviour {
    [SerializeField] private Toggle active;
    [SerializeField] private Slider intensity;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text intensityField;

    public void Toggle(bool on) {
        intensity.interactable = on;
        intensityField.color = on ? Color.white : Color.grey;
        title.color = intensityField.color;
    }

    public ChromaAbbSettings Into() {
        return new ChromaAbbSettings(active.isOn, intensity.value);
    }

    public void Init() {
        active.onValueChanged.AddListener(Toggle);
        active.isOn = Save.pData.ChromaAbbSettings.active;
        intensity.value = Save.pData.ChromaAbbSettings.intensity;
    }
}
