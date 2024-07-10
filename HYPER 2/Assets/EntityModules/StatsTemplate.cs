using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StatsTemplate))]
//[CanEditMultipleObjects]
public class StatsEditor : Editor {
    SerializedProperty stats;
    SerializedProperty values;

    void OnEnable()
    {
        stats = serializedObject.FindProperty("stats");
        values = serializedObject.FindProperty("values");
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

        serializedObject.ApplyModifiedProperties();
    }
}

public enum Stat {
    MAX_HEALTH,
    SPEED,
    DAMAGE,
    RANGE
}

[CreateAssetMenu(fileName = "Stats Template", menuName = "Mutox/Stats Template", order = 1)]
public class StatsTemplate : ScriptableObject {
    public Stat[] stats = new Stat[4] {
        Stat.MAX_HEALTH,
        Stat.SPEED,
        Stat.DAMAGE,
        Stat.RANGE
    };

    public float[] values = new float[4] {
        100f, 7f, 30f, 5f
    };
}
