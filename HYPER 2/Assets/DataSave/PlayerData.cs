using System;

[Serializable]
public class PlayerData
{
    public float __sensX = 2f;
    public float SensX {
        get { return __sensX; }
    }

    private void cSensX(float val) {
        __sensX = val;
    }

    private float __sensY = 2f;
    public float SensY {
        get { return __sensY; }
    }

    private void cSensY(float val) {
        __sensY = val;
    }

    public PlayerData(float sensX, float sensY) {
        __sensX = sensX;
        __sensY = sensY;
    }

    public PlayerData() {}

    
}
