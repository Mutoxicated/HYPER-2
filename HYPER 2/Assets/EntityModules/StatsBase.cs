using UnityEngine;
using System;
using Helpers;



#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(StatsBase))]
//[CanEditMultipleObjects]
public class BaseStatsEditor : Editor {
    SerializedProperty stats;
    SerializedProperty values;
    SerializedProperty statSide;

    void OnEnable()
    {
        stats = serializedObject.FindProperty("stats");
        values = serializedObject.FindProperty("values");
        statSide = serializedObject.FindProperty("side");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.BeginVertical();
        for (int i  = 0; i < Stt.EnumValues.Length; i++) {
            GUILayout.BeginHorizontal();
            if (i > values.arraySize-1) {
                values.InsertArrayElementAtIndex(i);
            }
            EditorGUILayout.PrefixLabel(Stt.EnumValues[i].ToString());
            //EditorGUILayout.PropertyField(modifiers.GetArrayElementAtIndex(i));
            EditorGUILayout.PropertyField(values.GetArrayElementAtIndex(i));
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.PropertyField(statSide);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif

[CreateAssetMenu(fileName = "Stats Base", menuName = "Mutox/Stats Base", order = 1)]
public class StatsBase : ScriptableObject {
    public float[] values =  new float[9] {
        100f, 7f, 30f, 5f, 1f, 1f, 1f, 100f, 1
    };

    public Stats.Side side;
}
