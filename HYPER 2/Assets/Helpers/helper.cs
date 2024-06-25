using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Option<T> {
    private T Value;
    public bool none = false;

    public Option(T val) {
        Value = val;
        none = true;
    }
    public Option() {}

    public bool IsNone() {
        return !none;
    }

    public T Get() {
        return Value;
    }
}

namespace Helpers {
    public class Vec3 {
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

    public class Vec2 {
        public static void PushTo(ref Vector2 v, float val) {
            if (v.x == 0) v.x = val;
            if (v.y == 0) v.y = val;
        }
    }

    public class MatType {
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

    public class Rdr {
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
