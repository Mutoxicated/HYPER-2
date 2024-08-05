using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
public class BloomSettings {
    public bool active = true;
    public float intensity = 2.69f;

    public BloomSettings() {}
}

[Serializable]
public class VignetteSettings {
    public bool active = true;
    public float intensity = 0.412f;
    public float smoothness = 0.736f;

    public VignetteSettings() {}
}


public class PlayerGraphics : MonoBehaviour 
{
    [SerializeField] private PostProcessVolume ppv;

    private Bloom bloom;
    private Vignette vignette;

    public void DStart() {
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