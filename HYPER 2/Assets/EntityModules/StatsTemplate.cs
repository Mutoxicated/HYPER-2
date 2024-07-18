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
    SerializedProperty element;
    SerializedProperty elementImmunities;
    SerializedProperty worth;
    SerializedProperty unlockAt;


    void OnEnable()
    {

        EditorUtility.SetDirty((StatsTemplate)target);
        statsBase = serializedObject.FindProperty("statsBase");
        mods = serializedObject.FindProperty("modifiers");
        element = serializedObject.FindProperty("element");
        worth = serializedObject.FindProperty("worth");
        unlockAt = serializedObject.FindProperty("unlockAt");
        elementImmunities = serializedObject.FindProperty("elementImmunities");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.PropertyField(statsBase);
        for (int i  = 0; i < Stt.EnumValues.Length; i++) {
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(Stt.EnumValues[i].ToString());
            //EditorGUILayout.PropertyField(modifiers.GetArrayElementAtIndex(i));
            if (i >= mods.arraySize) {
                mods.InsertArrayElementAtIndex(i);
            }
            EditorGUILayout.PropertyField(mods.GetArrayElementAtIndex(i));
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
        EditorGUILayout.PropertyField(worth);
        EditorGUILayout.PropertyField(unlockAt);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif

[Serializable]
public enum Stat {//
    MAX_HEALTH,
    SPEED,
    DAMAGE,
    RANGE,
    RATE
}

[CreateAssetMenu(fileName = "Stats Template", menuName = "Mutox/Stats Template", order = 1)]
public class StatsTemplate : ScriptableObject {
    public StatsBase statsBase;

    public List<float> modifiers = new List<float>() {
        1f, 1f, 1f, 1f, 1f
    };

    public Element element;
    public List<Element> elementImmunities = new List<Element>();
    public int worth = 1;
    public int unlockAt = 1;
}
