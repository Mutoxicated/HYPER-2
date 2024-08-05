using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class ChromaAbbSettings {
    public bool active = true;
    public float intensity = 0.5f;

    public ChromaAbbSettings() {}
}


public class ChromaAbb : MonoBehaviour
{
    [SerializeField] private Shader shader;

    private Material material;

	public void Start() {
        if (!Save.pData.ChromaAbbSettings.active) { enabled = false; }
        material = new Material(shader);
        UpdateMaterialProperties();
	}

    private void UpdateMaterialProperties() {
		material.SetFloat("_ChromaticAberration", 0.0025f * Save.pData.ChromaAbbSettings.intensity);
    }

	public void OnRenderImage(RenderTexture inTexture, RenderTexture outTexture) {
    	Graphics.Blit(inTexture, outTexture, material);
	}

    public void OnValidate() {
        if (material == null) return;
        UpdateMaterialProperties();
    }
}
