using System;
using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;
using static Stat;

// public enum Element {
//     FIRE,
//     RADIATION
// }


[Serializable]
public class health {
    [SerializeField] private float max;
    [HideInInspector] public float Current;

    public float Max {
        get { return max; }
    }

    public health(float max) {
        this.max = max;
        Current = max;
    }
}

[Serializable]
public class MTState {
    public List<Stat> modkey = new List<Stat>();
    public List<float> modVal = new List<float>();
    public Vector3 position;
    public health hp;
    public Resources.Arsenal arsenal;

    public MTState(Dictionary<Stat, float> mod, Vector3 pos, health hp, Resources.Arsenal arsenal) {
        (List<Stat>, List<float>) partition = General.DictToList(mod);
        modkey = partition.Item1;
        modVal = partition.Item2;
        position = pos;
        this.hp = hp;
        this.arsenal = arsenal;
    }
}

public class MiniTower : MonoBehaviour, IDamageable
{
    [SerializeField] private Stats stats;
    [SerializeField] private Interval DOTInterval;
    [SerializeField] private Resources.Arsenal arsenal;

    private health hp;
    private float DOT = .1f;

    public Stats Stats {
        get { return stats; }
    }

    public health HP {
        get { return hp; }
    }

    public Resources.Arsenal ArsenalType {
        get { return arsenal; }
    }

    private void StoreMTState(RunData r) {
        r.AddMTState(new MTState(Stats.mod, transform.position, hp, arsenal));
    }

    public void CStart(Manager manager) {
        PublicEvent.onSaveRun += StoreMTState;
        hp = stats.CStart(manager);

        gameObject.SetActive(true);
        DOTInterval.Enable();
    }

    public void CStart(Manager manager, MTState state) {
        PublicEvent.onSaveRun += StoreMTState;
        stats.CStart(manager);

        hp = state.hp;
        stats.mod = General.ListToDict(state.modkey, state.modVal);
        //transform.position = state.position;

        gameObject.SetActive(true);
        DOTInterval.Enable();
    }

    public void DOTIntervalDamage() {
        Damage(stats, DOT);
    }

    public void RestoreHP() {
        hp.Current = hp.Max;
    }

    public void Die(Option<Stats> _) {
        PublicEvent.onSaveRun -= StoreMTState;
        Destroy(gameObject);
    }

    public void Damage(Stats stats, float amount)
    {
        hp.Current -= amount;
        if (hp.Current <= 0) {
            Die(Option<Stats>.None());
        }
    }

    public void BacteriaDamage(BacteriaType bt, float amount) {
        hp.Current -= amount;
        if (hp.Current <= 0) {
            Die(Option<Stats>.None());
        }
    }
}
