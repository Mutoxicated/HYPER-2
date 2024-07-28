using System;
using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;
using static Stat;

[Serializable]
public class MTState {
    public List<Stat> modkey = new List<Stat>();
    public List<float> modVal = new List<float>();
    public Vector3 position;
    public float hp;
    public Resources.Arsenal arsenal;
    public int lvl = 1;
    public int xp = 0;

    public MTState(Dictionary<Stat, float> mod, Vector3 pos, float hp, Resources.Arsenal arsenal, int lvl, int xp) {
        (List<Stat>, List<float>) partition = General.DictToList(mod);
        modkey = partition.Item1;
        modVal = partition.Item2;
        position = pos;
        this.hp = hp;
        this.arsenal = arsenal;
        this.lvl = lvl;
        this.xp = xp;
    }
}

[RequireComponent(typeof(Stats))]
public class MiniTower : MonoBehaviour, ISideBelonger<MiniTower>
{
    [SerializeField] private Stats stats;
    [SerializeField] private Interval DOTInterval;
    [SerializeField] private Resources.Arsenal arsenal;
    [SerializeField, Range(0.01f, 1f)] private float DOT = .1f;
    [SerializeField] private Sound deathSound;

    private World.ReportDeath<MiniTower> death;

    public Stats Stats {
        get { return stats; }
    }

    public float HP {
        get { return stats.HP; }
    }

    public Resources.Arsenal ArsenalType {
        get { return arsenal; }
    }

    private void StoreMTState(RunData r) {
        r.AddMTState(new MTState(Stats.Mod, transform.position, stats.HP, arsenal, stats.LVL, stats.XP));
    }

    public void PrepareConfiguration() {
        stats.ConfigureOnStatDamage(true, (s, d) => {
            bool killed = stats.GetHit(d);
            return killed ? stats.Template.xpKillWorth : stats.Template.xpHitWorth;
        });
        stats.ConfigureOnExplosionDamage(true, (str, d) => {
            bool killed = stats.GetHit(d+(int)str);
            return killed ? stats.Template.xpKillWorth : stats.Template.xpHitWorth;
        });
        stats.ConfigureOnBacteriaDamage(true, (elm, d) => {
            bool killed = stats.GetHit(d);
            return killed ? stats.Template.xpKillWorth : stats.Template.xpHitWorth;
        });
    }

    public void CStart(Manager manager) {
        death = new World.ReportDeath<MiniTower>(_ => {});
        PublicEvent.onSaveRun += StoreMTState;
        stats.CStart(manager);
        PrepareConfiguration();

        gameObject.SetActive(true);
        DOTInterval.Enable();
    }

    public void CStart(Manager manager, MTState state) {
        death = new World.ReportDeath<MiniTower>(_ => {});
        PublicEvent.onSaveRun += StoreMTState;
        stats.CStart(manager, state);
        PrepareConfiguration();

        //transform.position = state.position;

        gameObject.SetActive(true);
        DOTInterval.Enable();
    }

    public void DOTIntervalDamage() {
        stats.Damage(stats, DOT);
    }

    public void RestoreHP() {
        stats.RestoreHP();
    }

    public void OnDisable() {
        PublicEvent.onSaveRun -= StoreMTState;
        death.Invoke(this);
        if (!gameObject.scene.isLoaded) { return; }
        deathSound.Play();
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