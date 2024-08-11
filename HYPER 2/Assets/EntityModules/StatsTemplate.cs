using UnityEngine;
using System;
using System.Collections.Generic;
using Helpers;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(StatsTemplate))]
//[CanEditMultipleObjects]
public class StatsEditor : Editor {
    SerializedProperty statsBase;
    SerializedProperty mods;
    SerializedProperty incrs;
    SerializedProperty element;
    SerializedProperty elementImmunities;
    SerializedProperty xpKillWorth;
    //SerializedProperty xpHitWorth;
    SerializedProperty maxXp;

    void OnEnable()
    {
        EditorUtility.SetDirty((StatsTemplate)target);
        statsBase = serializedObject.FindProperty("statsBase");
        mods = serializedObject.FindProperty("modifiers");
        incrs = serializedObject.FindProperty("incrementers");
        element = serializedObject.FindProperty("element");
        xpKillWorth = serializedObject.FindProperty("xpKillWorth");
        //xpHitWorth = serializedObject.FindProperty("xpHitWorth");
        elementImmunities = serializedObject.FindProperty("elementImmunities");
        maxXp = serializedObject.FindProperty("maxXp");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.PropertyField(statsBase);
        for (int i  = 0; i < Stt.EnumValues.Length; i++) {
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(Stt.EnumValues[i].ToString());
            //EditorGUILayout.PropertyField(modifiers.GetArrayElementAtIndex(i));
            if (i > mods.arraySize-1) {
                mods.InsertArrayElementAtIndex(i);
            }
            EditorGUILayout.PropertyField(mods.GetArrayElementAtIndex(i));
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.Separator();
        for (int i  = 0; i < Stt.EnumValues.Length; i++) {
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(Stt.EnumValues[i].ToString());
            //EditorGUILayout.PropertyField(modifiers.GetArrayElementAtIndex(i));
            if (i > incrs.arraySize-1) {
                incrs.InsertArrayElementAtIndex(i);
            }
            EditorGUILayout.PropertyField(incrs.GetArrayElementAtIndex(i));
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.PropertyField(element);
        EditorGUILayout.PropertyField(elementImmunities);
        // for (int i = 0; i < elementImmunities.arraySize; i++) {
        //     if (elementImmunities.GetArrayElementAtIndex(i).enumValueIndex == element.enumValueIndex) {
        //         elementImmunities.DeleteArrayElementAtIndex(i);
        //         break;
        //     }
        // }
        EditorGUILayout.PropertyField(maxXp);
        EditorGUILayout.PropertyField(xpKillWorth);
        //EditorGUILayout.PropertyField(xpHitWorth);
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
    }
}
#endif

[Serializable]
public enum Stat {
    MAX_HEALTH,
    SPEED,
    DAMAGE,
    RANGE,
    RATE,
    ARMOR,
    IMMUNITY,
    CHANCE,
    AMOUNT
}

[CreateAssetMenu(fileName = "Stats Template", menuName = "Mutox/Stats Template", order = 1), Serializable]
public class StatsTemplate : ScriptableObject {
    public StatsBase statsBase;

    public List<float> modifiers = new List<float>() {
        1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f
    };
    public List<float> incrementers = new List<float>() {
        0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f
    };
    
    public Element element;
    public List<Element> elementImmunities = new List<Element>();
    public int maxXp = 100;
    public int maxXpIncrememt = 20;
    public int xpKillWorth = 15;
}
