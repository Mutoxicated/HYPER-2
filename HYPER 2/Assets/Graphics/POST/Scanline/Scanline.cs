using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanline : MonoBehaviour
{
    [SerializeField] private Shader shader;
    [Range(0.001f, 10.0f)] public float lineSize = 1.0f;
    public int modulo = 2;

    private Material material;

	public void Start() {
        material = new Material(shader);
		UpdateMaterialProperties();
	}

    private void UpdateMaterialProperties() {
        		material.SetFloat("_LineSize", lineSize);
        material.SetInt("_Modulo", modulo);
    }

	public void OnRenderImage(RenderTexture inTexture, RenderTexture outTexture) {
        Graphics.Blit(inTexture, outTexture, material);
	}

    public void OnValidate() {
        if (material == null) return;
        UpdateMaterialProperties();
    }
}
