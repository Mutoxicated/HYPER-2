using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;

public class WireManifestation : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    [SerializeField] private float duration = 2f;
    [SerializeField] private float randomness = 0.1f;
    [SerializeField, Range(0.001f, 1f)] private float minIntact = 0.01f;
    
    private Material material;
    private float time = 0f;
    private float randomDuration;

    [ContextMenu("Test MinIntact")]
    private void TestMinIntact() {
        GetMaterial(true);
        material.SetFloat("_Intact", minIntact);
    }
    [ContextMenu("Reset Intact")]
    private void ResetIntact() {
        material.SetFloat("_Intact", 0f);
    }

    [ContextMenu("Get renderer")]
    private void GetRenderer() {
        _renderer = GetComponent<Renderer>();
    }

    private void GetMaterial(bool shared = false) {
        if (_renderer == null)
            GetRenderer();
        var res = shared ? Rdr.FindSharedMaterial(_renderer, "Wire") : Rdr.FindMaterial(_renderer, "Wire");
        if (res.IsNone()) {
            Debug.LogError("Couldn't find a wire material in "+gameObject.name+" (WireManifestation)");
            return;
        }
        material = res.Get();
    }

    private void OnEnable() {
        GetMaterial();
        color = material.color;
        color.a = 0f;
        material.color = color;
        material.SetFloat("_Intact", minIntact);
        randomDuration = duration+Random.Range(-randomness, randomness);
    }

    private Color color;
    private float linear;

    private void Update() {
        time += Time.deltaTime;
        linear = Mathf.InverseLerp(randomDuration, 0f, time);
        material.SetFloat("_Intact", Mathf.Lerp(0f, minIntact, linear));
        color = material.color;
        color.a = Mathf.Clamp01(1f-linear);
        material.color = color;


        if (time >= randomDuration) {
            time = 0f;
            material.SetFloat("_Intact", 0f);
            enabled = false;
        }
    }
}
