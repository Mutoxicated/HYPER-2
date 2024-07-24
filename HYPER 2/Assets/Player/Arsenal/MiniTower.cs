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

public class MiniTower : MonoBehaviour, IDamageable, ISideBelonger<MiniTower>
{
    [SerializeField] private Stats stats;
    [SerializeField] private Interval DOTInterval;
    [SerializeField] private Resources.Arsenal arsenal;
    [SerializeField, Range(0.01f, 1f)] private float DOT = .1f;

    private health hp;
    private World.ReportDeath<MiniTower> death;

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
        r.AddMTState(new MTState(Stats.Mod, transform.position, hp, arsenal));
    }

    public void CStart(Manager manager) {
        death = new World.ReportDeath<MiniTower>(_ => {});
        PublicEvent.onSaveRun += StoreMTState;
        hp = stats.CStart(manager);

        gameObject.SetActive(true);
        DOTInterval.Enable();
    }

    public void CStart(Manager manager, MTState state) {
        death = new World.ReportDeath<MiniTower>(_ => {});
        PublicEvent.onSaveRun += StoreMTState;
        hp = state.hp;
        stats.CStart(manager, state);

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

    public void OnDestroy() {
        PublicEvent.onSaveRun -= StoreMTState;
    }

    public void Die(Option<Stats> _) {
        death.Invoke(this);
        Destroy(gameObject);
    }

    public void Damage(Stats stats, float amount)
    {
        if (hp.Current < 0) { return; }

        hp.Current -= amount;
        if (hp.Current <= 0) {
            Die(Option<Stats>.None());
        }
    }

    public void BacteriaDamage(Element bt, float amount) {
        if (hp.Current < 0) { return; }
        
        hp.Current -= amount;
        if (hp.Current <= 0) {
            Die(Option<Stats>.None());
        }
    }

    public void ExplosionDamage(Explosion.Strength strength, float additive)
    {
        if (hp.Current < 0) { return; }
        
        hp.Current -= additive+(int)strength;
        if (hp.Current <= 0) {
            Die(Option<Stats>.None());
        }
    }

    public Stats.Side Side()
    {
        return stats.StatSide;
    }

    public GameObject Go()
    {
        return gameObject;
    }

    public bool IsNull() {
        return this == null;
    }


    public ref World.ReportDeath<MiniTower> GetReportDeath() {
        return ref death;
    }
}