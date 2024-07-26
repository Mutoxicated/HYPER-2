using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitVisual : MonoBehaviour
{
    [SerializeField] private MaterialChanger mc;
    [SerializeField, ColorUsage(true, true)] private Color hitColor = Color.white;
    [SerializeField, Range(0f,0.2f), Tooltip("")] private float rate;

    private float hitT = 0f;

    public void OnHit() {
        hitT = 1f;
        enabled = true;
    }

    private void LateUpdate() {
        hitT = Mathf.Clamp01(hitT-rate*Time.deltaTime);

        mc.ChangeColor(Color.Lerp(mc.Color, hitColor, hitT), false);

        if (hitT == 0f) {
            enabled = false;
        }
    }
}
