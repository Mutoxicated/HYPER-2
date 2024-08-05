using System;
using UnityEngine;

[Serializable]
public class PlayerData
{
    [SerializeField] private float __sens = 0.1f;
    
    public float Sens {
    	get { return __sens; }
    }
    
    public void SetSens(float val) {
    	__sens = val;
    }
    [SerializeField] private float __fov = 70f;
    
    public float Fov {
    	get { return __fov; }
    }
    
    public void SetFov(float val) {
    	__fov = val;
    }
    [SerializeField] private Color __uiColor = Color.white;
    
    public Color UIColor {
    	get { return __uiColor; }
    }
    
    public void SetUIColor(Color val) {
    	__uiColor = val;
    }


    // GRAPHICS
    [SerializeField] private ScalineSettings __scanlineSettings = new ScalineSettings();
        
    public ScalineSettings ScanlineSettings {
    	get { return __scanlineSettings; }
    }
    
    public void SetScalineSettings(ScalineSettings val) {
    	__scanlineSettings = val;
    }

    [SerializeField] private ChromaAbbSettings __chromaAbbSettings = new ChromaAbbSettings();
        
    public ChromaAbbSettings ChromaAbbSettings {
    	get { return __chromaAbbSettings; }
    }
    
    public void SetChromaAbb(ChromaAbbSettings val) {
    	__chromaAbbSettings = val;
    }

    [SerializeField] private BloomSettings __bloomSettings = new BloomSettings();
    
    public BloomSettings BloomSettings {
    	get { return __bloomSettings; }
    }
    
    public void SetBloomSettings(BloomSettings val) {
    	__bloomSettings = val;
    }

    [SerializeField] private VignetteSettings __vignetteSettings = new VignetteSettings();
    
    public VignetteSettings VignetteSettings {
    	get { return __vignetteSettings; }
    }
    
    public void SetVignetteSettings(VignetteSettings val) {
    	__vignetteSettings = val;
    }
    // SOUND
    [SerializeField] private float __masterVolume = 1f;

    public float MasterVolume {
        get { return __masterVolume; }
    }

    public void SetMasterVolume(float val) {
        __masterVolume = val;
    }

    [SerializeField] private float __musicVolume = 1f;

    public float MusicVolume {
        get { return __musicVolume; }
    }

    public void SetMusicVolume(float val) {
        __musicVolume = val;
    }

    [SerializeField] private float __sfxVolume = 1f;

    public float SFXVolume {
        get { return __sfxVolume; }
    }

    public void SetSFXVolume(float val) {
        __sfxVolume = val;
    }

    public PlayerData() {}
}
