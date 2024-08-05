using System;
using UnityEngine;

[Serializable]
public class PlayerData
{
    //MACRO_START
    [SerializeField] private float __sensX = 0.1f;
    
    public float SensX {
    	get { return __sensX; }
    }
    
    public void SetSensX(float val) {
    	__sensX = val;
    }
    [SerializeField] private float __sensY = 0.1f;
    
    public float SensY {
    	get { return __sensY; }
    }
    
    public void SetSensY(float val) {
    	__sensY = val;
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
    
    public void SetScanLineSize(float val) {
    	__scanlineSettings.lineSize = val;
    }

    public void SetScanlineModulo(int val) {
    	__scanlineSettings.modulo = val;
    }

    [SerializeField] private ChromaAbbSettings __chromaAbbSettings = new ChromaAbbSettings();
        
    public ChromaAbbSettings ChromaAbbSettings {
    	get { return __chromaAbbSettings; }
    }
    
    public void SetChromaAbbActive(bool val) {
    	__chromaAbbSettings.active = val;
    }

    public void SetChromaAbbIntensity(float val) {
    	__chromaAbbSettings.intensity = val;
    }

    [SerializeField] private BloomSettings __bloomSettings = new BloomSettings();
    
    public BloomSettings BloomSettings {
    	get { return __bloomSettings; }
    }
    
    public void SetBloomActive(bool val) {
    	__bloomSettings.active = val;
    }
    
    public void SetBloomIntensity(float val) {
    	__bloomSettings.intensity = val;
    }

    [SerializeField] private VignetteSettings __vignetteSettings = new VignetteSettings();
    
    public VignetteSettings VignetteSettings {
    	get { return __vignetteSettings; }
    }
    
    public void SetVignetteActive(bool val) {
    	__vignetteSettings.active = val;
    }
    
    public void SetVignetteIntensity(float val) {
    	__vignetteSettings.intensity = val;
    }

    public void SetVignetteSmoothness(float val) {
    	__vignetteSettings.intensity = val;
    }

    //MACRO_END

    public PlayerData(float sensX, float sensY, float fov) {
        __sensX = sensX;
        __sensY = sensY;
        __fov = fov;
    }

    public PlayerData() {}
}
