using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ChromaAbbSettings {
    public bool active = true;
    public float intensity = 0.5f;

    public ChromaAbbSettings() {}
    public ChromaAbbSettings(bool active, float intensity) {
        this.active = active;
        this.intensity = intensity;
    }
}

public class ChromaAbb : MonoBehaviour
{
    [SerializeField] private Shader shader;

    private Material material;

	public void DStart() {
        if (!Save.pData.ChromaAbbSettings.active) { enabled = false; }
        material = new Material(shader);
        UpdateMaterialProperties();
	}

    public void Refresh() {
        enabled = Save.pData.ChromaAbbSettings.active;
        UpdateMaterialProperties();
    }

    private void UpdateMaterialProperties() {
		material.SetFloat("_ChromaticAberration", 0.0025f * Save.pData.ChromaAbbSettings.intensity);
    }

	public void OnRenderImage(RenderTexture inTexture, RenderTexture outTexture) {
    	Graphics.Blit(inTexture, outTexture, material);
	}

    private void OnValidate() {
        if (material == null) return;
        UpdateMaterialProperties();
    }
}
