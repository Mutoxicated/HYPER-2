using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTower : MonoBehaviour
{
    [SerializeField] private Stats stats;
    [SerializeField] private Resources.EnemyTower enemyTower;
    // [SerializeField] private float tetherRange = 3f;
    // [SerializeField] private float tetherRangeInc = 0.1f;
    // [SerializeField] private Transform tetherPoint;

    [Header("Components")]
    [SerializeField] private Option<Shoot> shoot = Option<Shoot>.None();
    [SerializeField] private Option<AoE> aoe = Option<AoE>.None();

    public Option<Shoot> Shoot {
        get { return shoot; }
    }

    public Option<AoE> AOE {
        get { return aoe; }
    }

    private World.ReportDeath<EnemyTower> death;

    //private List<Resources.MTUpgradeType> upgrades = new List<Resources.MTUpgradeType>();

    public Stats Stats {
        get { return stats; }
    }

    public float HP {
        get { return stats.HP; }
    }

    // private void StoreMTState(RunData r) {
    //     r.AddMTState(new MTState(upgrades, transform.position, stats.HP, arsenal, stats.LVL, stats.XP));
    // }

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

    public void ConfigureTile(Tile tile) {
        death += m => { 
            if (tile != null) 
                tile.Deoccupy();
        };
        tile.Occupy(gameObject);
    }

    // private void ApplyUpgrades() {
    //     var upgrades = Save.rData.GetTakenUpgrades(arsenal);
    //     for (int i = 0; i < upgrades.Count; i++) {
    //         this.upgrades.Add(upgrades[i]);
    //         Resources.GetMTUpgrade(upgrades[i]).Action(this);
    //     }
    // }

    public void CStart(Manager manager, Tile tile) {
        death = new World.ReportDeath<EnemyTower>(_ => {});
        //PublicEvent.onSaveRun += StoreMTState;
        stats.CStart(manager, Resources.GetEnemyTowerStats(enemyTower));
        PrepareConfiguration();
        for (int i = 1; i < Gameplay.EnemyLvl; i++) {
            stats.IncXP(stats.MaxRequiredXP(), false);
        }
        ConfigureTile(tile);

        gameObject.SetActive(true);
    }

    public void CStart(Manager manager, MTState state, Tile tile) {
        death = new World.ReportDeath<EnemyTower>(_ => {});
        //PublicEvent.onSaveRun += StoreMTState;
        stats.CStart(manager, Resources.GetEnemyTowerStats(enemyTower), state);
        PrepareConfiguration();
        ConfigureTile(tile);

        //transform.position = state.position;

        gameObject.SetActive(true);
    }

    public void OnDisable() {
        //PublicEvent.onSaveRun -= StoreMTState;
        death.Invoke(this);
    }

    public ref World.ReportDeath<EnemyTower> GetReportDeath() {
        return ref death;
    }
}
