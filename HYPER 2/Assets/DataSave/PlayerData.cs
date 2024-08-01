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
    [SerializeField] private float __fov = 2f;
    
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

    //MACRO_END

    public PlayerData(float sensX, float sensY, float fov) {
        __sensX = sensX;
        __sensY = sensY;
        __fov = fov;
    }

    public PlayerData() {}
}
