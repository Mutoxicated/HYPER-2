using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helpers;

public enum MaterialType {
    BASE,
    SIM_WIRE,
    ADV_WIRE,
}

public class MaterialChanger : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    [SerializeField] private MaterialType @type;
    [SerializeField] private bool changeColor = true;
    [SerializeField, ColorUsage(true,true)] private Color color = Color.white;

    private Material material;

    public int @Type {
        set { 
            @Type = value; 
        }
        get { return @Type; }
    }

    public Color Color {
        set { color = value; }
        get { return color; }
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
}
