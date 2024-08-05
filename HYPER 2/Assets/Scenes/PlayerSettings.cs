using UnityEngine;
using UnityEngine.UI;

public interface ISettings {
    Doer Enable();
    void Disable();
    void SaveData();
}

public class PlayerSettings : MonoBehaviour, ISettings {
    [SerializeField] private Button button;
    [SerializeField] private Slider sens;
    [SerializeField] private Slider fov;
    [SerializeField] private Slider masterVolume;
    [SerializeField] private Slider musicVolume;
    [SerializeField] private Slider sfxVolume;

    public void DStart() {
        button.Start();
        button.SetTextColor(Color.grey);
        sens.value = Save.pData.Sens;
        fov.value = Save.pData.Fov;
        masterVolume.value = Save.pData.MasterVolume;
        musicVolume.value = Save.pData.MusicVolume;
        sfxVolume.value = Save.pData.SFXVolume;
    }

    public Doer Enable() {
        button.SetTextColor(Color.white);
        gameObject.SetActive(true);
        return () => {
            gameObject.SetActive(false);
            button.SetTextColor(Color.grey);
        };
    }

    public void Disable() {
        gameObject.SetActive(false);
        button.Disable();
    }

    public void SaveData() {
        Save.pData.SetSens(sens.value);
        Save.pData.SetFov(fov.value);
        Save.pData.SetMasterVolume(masterVolume.value);
        Save.pData.SetMusicVolume(musicVolume.value);
        Save.pData.SetSFXVolume(sfxVolume.value);
    }
}