using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Stats))]
//[CanEditMultipleObjects]
public class StatsEditor : Editor {
    SerializedProperty normalizeds;
    SerializedProperty values;
    SerializedProperty side;

    void OnEnable()
    {
        normalizeds = serializedObject.FindProperty("normalizeds");
        values = serializedObject.FindProperty("values");
        side = serializedObject.FindProperty("side");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.BeginVertical();
        for (int i  = 0; i < normalizeds.arraySize; i++) {
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(normalizeds.enumNames[i]);
            //EditorGUILayout.PropertyField(modifiers.GetArrayElementAtIndex(i));
            EditorGUILayout.PropertyField(values.GetArrayElementAtIndex(i));
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.PropertyField(side);
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}

public class Stats : MonoBehaviour
{
    public enum Side {
        PLAYER,
        ENEMY
    }

    public enum Value {
        MAX_HEALTH,
        SPEED,
        DAMAGE,
        RANGE
    }

    [SerializeField] private Value[] normalizeds = new Value[4] {
        Value.MAX_HEALTH,
        Value.SPEED,
        Value.DAMAGE,
        Value.RANGE
    };

    [SerializeField, Range(0f,2f)] private float[] modifiers = new float[4] {
        1f, 1f, 1f, 1f
    };

    [SerializeField] private float[] values = new float[4] {
        100f, 7f, 30f, 5f
    };

    [SerializeField] private Side side = Side.PLAYER; 

    public Dictionary<Value, float> mod = new Dictionary<Value, float>();
    public Dictionary<Value, float> val = new Dictionary<Value, float>();

    private Manager manager; // IMMUTABLE

    public Manager Manager {
        get { return manager; }
    }

    private Option<GameObject> target = Option<GameObject>.None();

    public GameObject Target {
        get { return target.Get(); }
    }

    private void PrepareValues() {
        for (int i = 0; i < normalizeds.Length; i++) {
            mod.Add(normalizeds[i], modifiers[i]);
        }
        for (int i = 0; i < normalizeds.Length; i++) {
            val.Add(normalizeds[i], values[i]);
        }
    }

    public health CStart(Manager manager) {
        this.manager = manager;
        PrepareValues();

        return new health(get(Value.MAX_HEALTH));
    }

    public float get(Value value) {
        return val[value]*mod[value];
    }

    private List<Enemy> closestEnemies;

    public bool TargetIsValid() {
        if (target.IsNone()) {
            return false;
        }

        return target.Get() != null;
    }

    public void SetTarget() {
        switch (side) {
        case Side.ENEMY:
            target = withinRange(manager.PlayerGo) ? new Option<GameObject>(manager.PlayerGo) : Option<GameObject>.None();
            break;
        case Side.PLAYER:
            closestEnemies = manager.World.GetClosestEnemies(transform, get(Value.RANGE));
            target = closestEnemies.Count != 0 ? new Option<GameObject>(closestEnemies[0].gameObject) : Option<GameObject>.None();
            break;
        }
    }

    public bool withinRange(GameObject go) {
        return Vector3.Distance(transform.position, go.transform.position) <= get(Value.RANGE);
    }
}
