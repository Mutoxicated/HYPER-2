using System;
using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;
using static Stat;

[Serializable]
public class MTState {
    public Vector3 position;
    public float hp;
    public Resources.Arsenal arsenal;
    public int lvl = 1;
    public int xp = 0;

    public MTState(Dictionary<Stat, float> mod, Vector3 pos, float hp, Resources.Arsenal arsenal, int lvl, int xp) {
        (List<Stat>, List<float>) partition = General.DictToList(mod);
        position = pos;
        this.hp = hp;
        this.arsenal = arsenal;
        this.lvl = lvl;
        this.xp = xp;
    }
}

public class MiniTower : MonoBehaviour
{
    [SerializeField] private Stats stats;
    [SerializeField] private Resources.Arsenal arsenal;
    [SerializeField, Range(0.01f, 1f)] private float DOT = .1f;
    [SerializeField] private float dotInterval = 0.2f;
    [SerializeField] private Highlight highlight;
    [SerializeField] private HPBarOptions options;
    [SerializeField] private Sound deathSound;

    private World.ReportDeath<MiniTower> death;
    private float time;

    public Stats Stats {
        get { return stats; }
    }

    public float HP {
        get { return stats.HP; }
    }

    public Resources.Arsenal ArsenalType {
        get { return arsenal; }
    }

    public void Highlight() {
        highlight.Enable();
    }

    public void Dehighlight() {
        highlight.Disable();
    }

    private void StoreMTState(RunData r) {
        r.AddMTState(new MTState(Stats.Mod, transform.position, stats.HP, arsenal, stats.LVL, stats.XP));
    }

    public void PrepareConfiguration() {
        stats.ConfigureOnStatDamage(true, (s, d) => {
            bool killed = stats.GetHit(d);
            return killed ? stats.Template.xpKillWorth : 0;
        });
        stats.ConfigureOnExplosionDamage(true, (str, d) => {
            bool killed = stats.GetHit(d+(int)str);
            return killed ? stats.Template.xpKillWorth : 0;
        });
        stats.ConfigureOnBacteriaDamage(true, (elm, d) => {
            bool killed = stats.GetHit(d);
            return killed ? stats.Template.xpKillWorth : 0;
        });
    }

    private void SetHPBar() {
        stats.Manager.GUI.HPBP.GetHPBar(options, stats);
    }

    public void CStart(Manager manager) {
        death = new World.ReportDeath<MiniTower>(_ => {});
        PublicEvent.onSaveRun += StoreMTState;
        stats.CStart(manager, Resources.GetStatTemplate(arsenal));
        PrepareConfiguration();
        for (int i = 1; i < Save.rData[arsenal, true]; i++) {
            stats.IncXP(stats.MaxRequiredXP(), false);
        }
        SetHPBar();

        gameObject.SetActive(true);
    }

    public void CStart(Manager manager, MTState state) {
        death = new World.ReportDeath<MiniTower>(_ => {});
        PublicEvent.onSaveRun += StoreMTState;
        stats.CStart(manager, Resources.GetStatTemplate(arsenal), state);
        PrepareConfiguration();
        SetHPBar();

        //transform.position = state.position;

        gameObject.SetActive(true);
    }

    private void Update() {
        time += Time.deltaTime;
        if (time >= dotInterval) {
            time = 0f;
            DOTIntervalDamage();
        }
    }

    public void DOTIntervalDamage() {
        stats.Damage(stats, DOT);
    }

    public void RestoreHP() {
        stats.RestoreHP();
    }

    public void OnDisable() {
        Dehighlight();
        PublicEvent.onSaveRun -= StoreMTState;
        death.Invoke(this);
        if (!gameObject.scene.isLoaded) { return; }
        deathSound.Play();
    }

    public ref World.ReportDeath<MiniTower> GetReportDeath() {
        return ref death;
    }
}