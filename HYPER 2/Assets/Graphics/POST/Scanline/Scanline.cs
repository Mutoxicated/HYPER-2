using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class ScalineSettings {
    public bool active = true;
    public float lineSize = 1f;
    public int modulo = 2;

    public ScalineSettings() {}
}

public class Scanline : MonoBehaviour
{
    [SerializeField] private Shader shader;

    private Material material;

	public void Start() {
        if (!Save.pData.ScanlineSettings.active) { enabled = false; }
        material = new Material(shader);
		UpdateMaterialProperties();
	}

    private void UpdateMaterialProperties() {
        material.SetFloat("_LineSize", Save.pData.ScanlineSettings.lineSize);
        material.SetInt("_Modulo", Save.pData.ScanlineSettings.modulo);
    }

	public void OnRenderImage(RenderTexture inTexture, RenderTexture outTexture) {
        Graphics.Blit(inTexture, outTexture, material);
	}

    public void OnValidate() {
        if (material == null) return;
        UpdateMaterialProperties();
    }
}
