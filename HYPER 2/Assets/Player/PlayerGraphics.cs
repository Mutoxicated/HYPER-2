using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

[Serializable]
public class BloomSettings {
    public bool active = true;
    public float intensity = 2.69f;

    public BloomSettings() {}
    public BloomSettings(bool active, float intensity) {
        this.active = active; 
        this.intensity = intensity;
    }
}

[Serializable]
public class VignetteSettings {
    public bool active = true;
    public float intensity = 0.412f;
    public float smoothness = 0.736f;

    public VignetteSettings() {}
    public VignetteSettings(bool active, float intensity, float smoothness) {
        this.active = active;
        this.intensity = intensity;
        this.smoothness = smoothness;
    }
}

public class PlayerGraphics : MonoBehaviour 
{
    [SerializeField] private PostProcessVolume ppv;
    [SerializeField] private Scanline scanline;
    [SerializeField] private ChromaAbb chromaAbb;

    private Bloom bloom;
    private Vignette vignette;

    public void DStart() {
        scanline.DStart();
        chromaAbb.DStart();
        Refresh();
    }

    public void Refresh() {
        scanline.Refresh();
        chromaAbb.Refresh();
        var profile = ppv.profile;
        profile.TryGetSettings(out bloom);
        profile.TryGetSettings(out vignette);

        bloom.active = Save.pData.BloomSettings.active;
        bloom.intensity.value = Save.pData.BloomSettings.intensity;

        var vignetteSettings = Save.pData.VignetteSettings;

        vignette.active = vignetteSettings.active;
        vignette.intensity.value = vignetteSettings.intensity;
        vignette.smoothness.value = vignetteSettings.smoothness;
    }
}