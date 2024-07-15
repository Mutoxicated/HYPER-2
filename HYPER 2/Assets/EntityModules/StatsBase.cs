using UnityEngine;
using System;


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
        for (int i  = 0; i < stats.arraySize; i++) {
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(stats.enumNames[i]);
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
    public Stat[] stats = new Stat[4] {
        Stat.MAX_HEALTH,
        Stat.SPEED,
        Stat.DAMAGE,
        Stat.RANGE
    };

    public float[] values = new float[4] {
        100f, 7f, 30f, 5f
    };

    public Stats.Side side;
}
