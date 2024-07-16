using System.Collections;
using System.Collections.Generic;
using Helpers;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public enum Element {
    WATER,
    FIRE,
    RADIATION
}

public class Stats : MonoBehaviour
{
    public enum Side {
        PLAYER,
        ENEMY
    }

    [SerializeField] private StatsTemplate template;

    [SerializeField] private Option<Range> range = Option<Range>.None();

    private Dictionary<Stat, float> mod = new Dictionary<Stat, float>();
    private Dictionary<Stat, float> val = new Dictionary<Stat, float>();

    private Manager manager; // IMMUTABLE

    public Dictionary<Stat, float> Mod {
        set { mod = value; }
        get { return mod; }
    }

    public Manager Manager {
        get { return manager; }
    }

    public StatsTemplate Template {
        get { return template; }
    }

    public Option<GameObject> Target(int offset = 0) {
        return range.Get().Target(offset);
    }

    public Side StatSide {
        get { return template.statsBase.side; }
    }

    public Element StatElement {
        get { return template.element; }
    }

    public Option<Range> Range {
        get { return range; }
    }

    private void PrepareValues() {
        for (int i = 0; i < template.stats.Length; i++) {
            mod.Add(template.stats[i], template.modifiers[i]);
        }
        for (int i = 0; i < template.stats.Length; i++) {
            val.Add(template.stats[i], template.statsBase.values[i]);
        }
    }

    public health CStart(Manager manager) {
        this.manager = manager;
        PrepareValues();
        range.GetAndThen(v => v.CStart());

        return new health(get(Stat.MAX_HEALTH));
    }

    public void CStart(Manager manager, MTState state) {
        this.manager = manager;
        mod = General.ListToDict(state.modkey, state.modVal);
        for (int i = 0; i < template.stats.Length; i++) {
            val.Add(template.stats[i], template.statsBase.values[i]);
        }
        range.GetAndThen(v => v.CStart());
    }

    public float get(Stat value) {
        return val[value]*mod[value];
    }

    public void inc(Stat stat, float value) {
        mod[stat] += value;
    }

    public bool TargetIsValid(int offset = 0) {
        if (range.IsNone()) {
            return false;
        }
        return range.Get().Target(offset).IsSome();
    }

    public void Inherit(Stats other) {
        foreach (var k in mod.Keys) {
            mod[k] *= other.mod[k];
        }
    }
}
