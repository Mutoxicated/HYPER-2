using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;
using AYellowpaper;
using Unity.VisualScripting;

public enum Element {
    WATER,
    FIRE,
    RADIATION,
    AIR
}

public class Stats : MonoBehaviour
{
    public enum Side {
        PLAYER,
        ENEMY
    }

    [SerializeField] private StatsTemplate template;

    [SerializeField] private Option<InterfaceReference<IRange, MonoBehaviour>> range;

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
        return range.Get().Value.Target(offset);
    }

    public Side StatSide {
        get { return template.statsBase.side; }
    }

    public Element StatElement {
        get { return template.element; }
    }

    public List<Element> StatElmImmunities {
        get { return template.elementImmunities; }
    }

    public Option<InterfaceReference<IRange, MonoBehaviour>> Range {
        get { return range; }
    }

    private void PrepareValues() {
        for (int i = 0; i < Stt.EnumValues.Length; i++) {
            if (i >= template.modifiers.Count) {
                template.modifiers.Add(1f);
            }
            mod.Add(Stt.EnumValues[i], template.modifiers[i]);
        }
        for (int i = 0; i < Stt.EnumValues.Length; i++) {
            val.Add(Stt.EnumValues[i], template.statsBase.values[i]);
        }
    }

    public health CStart(Manager manager) {
        this.manager = manager;
        PrepareValues();
        range.GetAndThen(v => v.Value.CStart());

        return new health(get(Stat.MAX_HEALTH));
    }

    public void CStart(Manager manager, MTState state) {
        this.manager = manager;
        mod = General.ListToDict(state.modkey, state.modVal);
        for (int i = 0; i < Stt.EnumValues.Length; i++) {
            val.Add(Stt.EnumValues[i], template.statsBase.values[i]);
        }
        range.GetAndThen(v => v.Value.CStart());
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
        return range.Get().Value.Target(offset).IsSome();
    }

    public void Inherit(Stats other) {
        foreach (var k in mod.Keys) {
            mod[k] *= other.mod[k];
        }
    }

    public bool ImmuneTo(Element elm) {
        return StatElmImmunities.Contains(elm);
    }
}
