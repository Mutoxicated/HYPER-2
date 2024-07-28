using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitVisual : MonoBehaviour
{
    [SerializeField] private MaterialChanger mc;
    [SerializeField, ColorUsage(true, true)] private Color hitColor = Color.white;
    [SerializeField, Range(0f,0.4f), Tooltip("")] private float rate = 0.4f;

    private float hitT = 0f;

    public void OnHit(float tMod = 1f) {
        hitT = tMod;
        enabled = true;
    }

    private void LateUpdate() {
        hitT = Mathf.Clamp01(hitT-rate*4*Time.deltaTime);

        mc.ChangeColor(Color.Lerp(mc.Color, hitColor, hitT), true);

        if (hitT == 0f) {
            enabled = false;
        }
    }
}
