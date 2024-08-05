using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public class ScalineSettings {
    public bool active = true;
    public float lineSize = 1f;
    public int modulo = 2;

    public ScalineSettings() {}
    public ScalineSettings(bool active, float lineSize, int modulo) {
        this.active = active;
        this.lineSize = lineSize;
        this.modulo = modulo;
    }
}

public class Scanline : MonoBehaviour
{
    [SerializeField] private Shader shader;

    private Material material;

	public void DStart() {
        if (!Save.pData.ScanlineSettings.active) { enabled = false; }
        material = new Material(shader);
		UpdateMaterialProperties();
	}

    public void Refresh() {
        enabled = Save.pData.ScanlineSettings.active;
        UpdateMaterialProperties();
    }

    private void UpdateMaterialProperties() {
        material.SetFloat("_LineSize", Save.pData.ScanlineSettings.lineSize);
        material.SetInt("_Modulo", Save.pData.ScanlineSettings.modulo);
    }

	public void OnRenderImage(RenderTexture inTexture, RenderTexture outTexture) {
        Graphics.Blit(inTexture, outTexture, material);
	}

    private void OnValidate() {
        if (material == null) return;
        UpdateMaterialProperties();
    }
}
