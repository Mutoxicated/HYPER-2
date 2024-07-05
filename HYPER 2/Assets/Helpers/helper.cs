using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Option<T> {
    private T value;
    private bool none = true;

    public Option(T val) {
        value = val;
        none = false;
    }
    private Option() {}

    public bool IsNone() {
        return none;
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
        private static Dictionary<EditorLayer, string> Layers = new Dictionary<EditorLayer, string>() {
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

    public static class Dir {
        private static Dictionary<Direction, Vector3> directions = new Dictionary<Direction, Vector3>() {
            { Direction.DOWN, Vector3.back },
            { Direction.UP, Vector3.forward },
            { Direction.RIGHT, Vector3.right },
            { Direction.LEFT, Vector3.left },
            { Direction.UNKNOWN, Vector3.zero }
        };

        public static Vector3 ToVec(Direction dir) {
            return directions[dir];
        } 

        public static Direction FromVec(Vector3 dir) {
            dir.Normalize();
            dir = Vec3.Round(dir);

            Debug.Log(dir);

            if (dir == Vector3.back) {
                return Direction.DOWN;
            }else if (dir == Vector3.forward) {
                return Direction.UP;
            }else if (dir == Vector3.right) {
                return Direction.RIGHT;
            }else if (dir == Vector3.left) {
                return Direction.LEFT;
            }

            return Direction.UNKNOWN;
        }
    }

    public static class Go {
        public static void SetLayerRecursive(GameObject go, EditorLayer el) {
            int layer = EditorState.GetLayer(el);
            go.layer = layer;
            for (int i = 0; i < go.transform.childCount; i++) {
                LayerRecurse(go.transform.GetChild(i).gameObject, layer);
            }
        }

        private static void LayerRecurse(GameObject go, int layer) {
            go.layer = layer;
            for (int i = 0; i < go.transform.childCount; i++) {
                LayerRecurse(go.transform.GetChild(i).gameObject, layer);
            }
        }
    }

    public static class Num {
        public static int Wrap(int value, int min, int max) {
            if (value > max) {
                //Debug.Log("value: "+value);
                float length = max-min;
                //Debug.Log("length: "+length);
                int times = Mathf.CeilToInt(value/length); 
                //Debug.Log("times: "+times);
                return Mathf.FloorToInt(length)*times-value;
            }else {
                return value;
            }
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

        public static Vector3 Round(Vector3 v) {
            v.x = Mathf.Round(v.x);
            v.y = Mathf.Round(v.y);
            v.z = Mathf.Round(v.z);

            return v;
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
            return Option<Material>.None();
        }
    }
}
