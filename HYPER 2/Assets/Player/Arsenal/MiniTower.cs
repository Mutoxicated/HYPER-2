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
    public List<Resources.MTUpgradeType> upgrades = new List<Resources.MTUpgradeType>();

    public MTState(List<Resources.MTUpgradeType> upgrades, Vector3 pos, float hp, Resources.Arsenal arsenal, int lvl, int xp) {
        this.upgrades = upgrades;
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
    // [SerializeField] private float tetherRange = 3f;
    // [SerializeField] private float tetherRangeInc = 0.1f;
    // [SerializeField] private Transform tetherPoint;
    [SerializeField, Range(0.01f, 1f)] private float DOT = .1f;
    [SerializeField] private float dotInterval = 0.2f;
    [SerializeField] private Highlight highlight;
    [SerializeField] private HPBarOptions options;
    [SerializeField] private Sound deathSound;

    [Header("Components")]
    [SerializeField] private Option<Shoot> shoot = Option<Shoot>.None();
    [SerializeField] private Option<AoE> aoe = Option<AoE>.None();

    public Option<Shoot> Shoot {
        get { return shoot; }
    }

    public Option<AoE> AOE {
        get { return aoe; }
    }

    private World.ReportDeath<MiniTower> death;
    private float time;

    private List<Resources.MTUpgradeType> upgrades = new List<Resources.MTUpgradeType>();
    private MinitowerInfo mtInfo;

    public MinitowerInfo MTInfo {
        get { return mtInfo; }
    }
    
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

    public MTState State() {
        return new MTState(upgrades, transform.position, stats.HP, arsenal, stats.LVL, stats.XP);
    }

    public void PrepareConfiguration() {
        stats.ConfigureOnStatDamage(true, (s, d) => {
            bool killed = stats.GetHit(new Option<Stats>(s), d);
            return killed ? stats.Template.xpKillWorth : 0;
        });
        stats.ConfigureOnExplosionDamage(true, (str, d) => {
            bool killed = stats.GetHit(Option<Stats>.None(), d+(int)str);
            return killed ? stats.Template.xpKillWorth : 0;
        });
        stats.ConfigureOnBacteriaDamage(true, (elm, d) => {
            bool killed = stats.GetHit(Option<Stats>.None(), d);
            return killed ? stats.Template.xpKillWorth : 0;
        });
    }

    private void SetHPBar() {
        stats.Manager.GUI.HPBP.GetHPBar(options, stats);
    }

    public void ConfigureTile(Tile tile) {
        death += m => { 
            if (tile != null) 
                tile.Deoccupy();
        };
        tile.Occupy(gameObject);
    }

    private void ApplyUpgrades() {
        var upgrades = Save.rData.TakenUpgrades[(int)arsenal].self;
        for (int i = 0; i < upgrades.Count; i++) {
            this.upgrades.Add(upgrades[i]);
            Resources.GetMTUpgrade(upgrades[i]).Action(this);
        }
    }

    public void CStart(Manager manager, Tile tile) {
        death = new World.ReportDeath<MiniTower>(_ => {});
        mtInfo = Resources.GetMTInfo(arsenal);
        stats.CStart(manager, Resources.GetStatTemplate(arsenal));
        PrepareConfiguration();
        for (int i = 1; i < Save.rData.ResearchLvls[(int)arsenal]; i++) {
            stats.IncXP(stats.MaxRequiredXP(), false);
        }
        SetHPBar();
        ConfigureTile(tile);
        ApplyUpgrades();

        gameObject.SetActive(true);
    }

    public void CStart(Manager manager, MTState state, Tile tile) {
        death = new World.ReportDeath<MiniTower>(_ => {});
        mtInfo = Resources.GetMTInfo(arsenal);
        stats.CStart(manager, Resources.GetStatTemplate(arsenal), state);
        PrepareConfiguration();
        SetHPBar();
        ConfigureTile(tile);
        upgrades = state.upgrades;
        for (int i = 0; i < upgrades.Count; i++) {
            Resources.GetMTUpgrade(upgrades[i]).Action(this);
        }

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
        //stats.Damage(stats, DOT);
    }

    public void RestoreHP() {
        stats.RestoreHP();
    }

    public void OnDisable() {
        if (!gameObject.scene.isLoaded) { return; }
        Dehighlight();
        death.Invoke(this);
        deathSound.Play();
    }

    public void AddOnDeath(World.ReportDeath<MiniTower> onDeath) {
        death += onDeath;
    }
}