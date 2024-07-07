using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Stats))]
//[CanEditMultipleObjects]
public class StatsEditor : Editor {
    SerializedProperty normalizeds;
    SerializedProperty values;
    
    void OnEnable()
    {
        normalizeds = serializedObject.FindProperty("normalizeds");
        values = serializedObject.FindProperty("values");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.BeginVertical();
        for (int i  = 0; i < normalizeds.arraySize; i++) {
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(normalizeds.enumNames[i]);
            EditorGUILayout.PropertyField(values.GetArrayElementAtIndex(i));
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}

public class Stats : MonoBehaviour
{
    public enum Normalized {
        HEALTH,
        SPEED,
        DAMAGE,
        RANGE
    }

    [SerializeField] private Normalized[] normalizeds = new Normalized[4] {
        Normalized.HEALTH,
        Normalized.SPEED,
        Normalized.DAMAGE,
        Normalized.RANGE
    };

    [SerializeField, Range(0f,2f)] private float[] values = new float[4] {
        1f, 1f, 1f, 1f
    };

    [HideInInspector] public Dictionary<Normalized, float> get = new Dictionary<Normalized, float>();

    public void CStart() {
        for (int i = 0; i < normalizeds.Length; i++) {
            get.Add(normalizeds[i], values[i]);
        }
    }
}
