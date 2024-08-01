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
    PIXEL_OBJECT,
    OUTLINE
}

public enum QueueMode {
    GEOMETRY = 2000,
    ALPHA_TEST = 2500,
    TRANSPARENT = 3000
}

public class MaterialChanger : MonoBehaviour
{
    [Header("Means of access to material")]
    [SerializeField] private Renderer _renderer;
    [SerializeField] private SkinnedMeshRenderer smr;
    [SerializeField] private MaterialType @type;
    [Header("Settings")]
    [SerializeField] private bool changeColor = true;
    [SerializeField, ColorUsage(true,true)] private Color color = Color.white;
    [SerializeField] private bool changeQueue = false;
    [SerializeField] private QueueMode queueMode;
    [SerializeField] private int offset;

    [HideInInspector] public Material material;

    public int @Type {
        set { 
            @Type = value; 
        }
        get { return @Type; }
    }

    public Color Color {
        get { return color; }
    }

    void Awake()
    {
        _renderer = _renderer == null ? GetComponent<Renderer>() : _renderer; 
        if (_renderer == null)
            smr = smr == null ? GetComponent<SkinnedMeshRenderer>() : smr;
        
        Option<Material> res = Option<Material>.None();
        if (_renderer != null) {
            res = Rdr.FindMaterial(_renderer, @type);
        }else if (smr != null) {
            res = Rdr.FindMaterial(smr, @type);
        }
        if (res.IsNone()) {
            Debug.LogError("Couldn't find a material of type "+@type, _renderer);
            return;
        }
        material = res.Get();

        ChangeColor(color);
        ChangeQueue(queueMode+offset, false);
    }

    public void CStart() {
        Awake();
    }

    public void ChangeColor(Color color) {
        if (changeColor) {
            material.color = color;
        }
    }
    public void ChangeColorOverride(Color color) {
        material.color = color;
    }
    public void ChangeColor() {
        if (changeColor) {
            material.color = color;
        }
    }
    public void ChangeColorOverride() {
        material.color = color;
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

    public void SetColor(Color color) {
        this.color = color;
    }
}
