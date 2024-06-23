using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChromaAbb : MonoBehaviour
{
    [SerializeField] private Shader shader;
	[Range(0.0f, 1.0f)] public float chromaticAberration = 1.0f;

    private Material material;

	public void Start() {
        material = new Material(shader);
        UpdateMaterialProperties();
	}

    private void UpdateMaterialProperties() {
		material.SetFloat("_ChromaticAberration", 0.01f * chromaticAberration);
    }

	public void OnRenderImage(RenderTexture inTexture, RenderTexture outTexture) {
    	Graphics.Blit(inTexture, outTexture, material);
	}

    public void OnValidate() {
        if (material == null) return;
        UpdateMaterialProperties();
    }
}
