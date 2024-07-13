using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(StatsTemplate))]
//[CanEditMultipleObjects]
public class StatsEditor : Editor {
    SerializedProperty stats;
    SerializedProperty values;
    SerializedProperty statSide;
    SerializedProperty element;
    SerializedProperty worth;
    SerializedProperty unlockAt;

    void OnEnable()
    {
        stats = serializedObject.FindProperty("stats");
        values = serializedObject.FindProperty("values");
        statSide = serializedObject.FindProperty("side");
        element = serializedObject.FindProperty("element");
        worth = serializedObject.FindProperty("worth");
        unlockAt = serializedObject.FindProperty("unlockAt");
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
        EditorGUILayout.PropertyField(element);
        EditorGUILayout.PropertyField(worth);
        EditorGUILayout.PropertyField(unlockAt);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif

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

    public Stats.Side side;
    public Stats.Element element;
    public int worth = 1;
    public int unlockAt = 1;
}
