using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helpers;

public enum MaterialType {
    BASE,
    SIM_WIRE,
    ADV_WIRE,
    HDR,
    SELECTION,
    PIXEL_OBJECT
}

public enum QueueMode {
    GEOMETRY = 2000,
    ALPHA_TEST = 2500,
    TRANSPARENT = 3000
}

public class MaterialChanger : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    [SerializeField] private MaterialType @type;
    [SerializeField] private bool changeColor = true;
    [SerializeField, ColorUsage(true,true)] private Color color = Color.white;
    [SerializeField] private bool changeQueue = false;
    [SerializeField] private QueueMode queueMode;
    [SerializeField] private int offset;

    public Material material;

    public int @Type {
        set { 
            @Type = value; 
        }
        get { return @Type; }
    }

    public ref Color Color {
        get { return ref color; }
    }

    void Awake()
    {
        _renderer = _renderer == null? GetComponent<Renderer>() : _renderer; 
        var res = Rdr.FindMaterial(_renderer, @type);
        if (res.IsNone()) {
            Debug.LogError("Couldn't find a material of type "+@type, _renderer);
            return;
        }
        material = res.Get();

        ChangeColor(color, false);
        ChangeQueue(queueMode+offset, false);
    }

    public void CStart() {
        Awake();
    }

    public void ChangeColor(Color color, bool overrideSettings) {
        if (overrideSettings) {
            material.color = color;
            return;
        }
        if (changeColor) {
            material.color = color;
        }
    }

    public void ChangeColor(bool overrideSettings) {
        ChangeColor(color, overrideSettings);
    }

    public void ChangeQueue(QueueMode mode, bool overrideSettings) {
        if (overrideSettings) {
            material.renderQueue = (int)mode;
            return;
        }
        if (changeQueue) {
            material.renderQueue = (int)mode;
        }
    }
}
