using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Option<T> {
    private T value;
    private bool none = false;

    public Option(T val) {
        value = val;
        none = true;
    }
    public Option() {}

    public bool IsNone() {
        return !none;
    }

    public bool IsSome() {
        return !IsNone();
    }

    public T Get() {
        return value;
    }

    public static Option<T> None() {
        return new Option<T>();
    }
}

namespace Helpers {
    public enum EditorLayer {
        DEFAULT,
        POST,
        INTERACTABLE_ONE,
        INTERACTABLE_TWO
    }

    public static class EditorState {
        public static Dictionary<EditorLayer, string> Layers = new Dictionary<EditorLayer, string>() {
            {EditorLayer.DEFAULT, "Default"},
            {EditorLayer.POST, "Post"},
            {EditorLayer.INTERACTABLE_ONE, "Interactable1"},
            {EditorLayer.INTERACTABLE_TWO, "Interactable2"},
        };

        public static LayerMask GetLayer(EditorLayer et) {
            return LayerMask.NameToLayer(Layers[et]);
        }
    }

    public static class General {
        public static T FiftyFifty<T>(T v1, T v2) {
            return Seed.Chance(50f) ? v1 : v2;
        }
    }

    public static class Vec3 {
        public static void Ceil(ref Vector3 v) {
            v.x = Mathf.Ceil(v.x);
            v.y = Mathf.Ceil(v.y);
            v.z = Mathf.Ceil(v.z);
        }

        public static Vector2 ToV2XZ(Vector3 v) {
            return new Vector2(v.x, v.z);
        } 

        public static bool IsPositive(Vector3 v) {
            if (v.x < 0) return false;
            if (v.y < 0) return false;
            if (v.z < 0) return false;
            return true;
        }

        public static bool IsNegative(Vector3 v) {
            return !IsPositive(v);
        }
    }

    public static class Vec2 {
        public static void PushTo(ref Vector2 v, float val) {
            if (v.x == 0) v.x = val;
            if (v.y == 0) v.y = val;
        }
    }

    public static class MatType {
        public static string Name(MaterialType mt) {
            switch (mt) {
            case MaterialType.BASE:
                return "Base";
            case MaterialType.SIM_WIRE:
                return "SimpleWire";
            case MaterialType.ADV_WIRE:
                return "AdvancedWire";
            default:
                return "UNKNOWN";
            }
        }
    }

    public static class Rdr {
        public static Option<Material> FindMaterial(Renderer r, MaterialType mt) {
            var name = MatType.Name(mt);
            foreach (Material m in r.materials) {
                if (m.name.Contains(name)) {
                    return new Option<Material>(m);
                }
            }
            return new Option<Material>();
        }
    }
}
