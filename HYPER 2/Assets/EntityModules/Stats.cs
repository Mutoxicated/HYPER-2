using System.Collections;
using System.Collections.Generic;
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

    /// <summary>
    /// Access this only if you are sure that there are targets to choose
    /// </summary>
    public GameObject Target {
        get { return range.Get().Target().Get(); }
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

    public float get(Stat value) {
        return val[value]*mod[value];
    }

    public bool TargetIsValid() {
        if (range.IsNone()) {
            return false;
        }
        return range.Get().Target().IsSome();
    }

    public void Inherit(Stats other) {
        val = other.val;
    }
}
