using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;
using AYellowpaper;
using Unity.VisualScripting;
using System.Linq;

public enum Element {
    WATER,
    FIRE,
    RADIATION,
    AIR
}

public class Stats : MonoBehaviour, IDamageable
{
    public enum Side {
        PLAYER,
        ENEMY
    }

    [SerializeField] private StatsTemplate template;

    [SerializeField] private Option<InterfaceReference<IRange, MonoBehaviour>> range;

    private Dictionary<Stat, float> mod = new Dictionary<Stat, float>();
    private Dictionary<Stat, float> val = new Dictionary<Stat, float>();
    private readonly Dictionary<Stat, float> incrs = new Dictionary<Stat, float>();

    private float hp;
    private int xp = 0;
    private int lvl = 1;

    private Manager manager; // IMMUTABLE
    public delegate void OnDie(Option<Stats> stats);
    public delegate void OnStatDamage(Stats stats, float amount);
    public delegate void OnBacteriaDamage(Element elm, float amount);
    public delegate void OnExplosionDamage(Explosion.Strength strength, float additive);

    public OnDie onDie;
    public OnStatDamage onStatDamage;
    public OnBacteriaDamage onBacteriaDamage;
    public OnExplosionDamage onExplosionDamage;

    public Dictionary<Stat, float> Mod {
        set { mod = value; }
        get { return mod; }
    }

    public float HP {
        get { return hp; }
    }

    public int XP {
        get { return xp; }
    }

    public int LVL {
        get { return lvl; }
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

    public void RestoreHP() {
        hp = get(Stat.MAX_HEALTH);
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
        for (int i = 0; i < Stt.EnumValues.Length; i++) {
            if (i >= template.incrementers.Count) {
                template.incrementers.Add(0.1f);
            }
            incrs.Add(Stt.EnumValues[i], template.incrementers[i]);
        }
        RestoreHP();
    }

    public void CStart(Manager manager) {
        this.manager = manager;
        PrepareValues();
        range.GetAndThen(v => v.Value.CStart());
    }

    public void CStart(Manager manager, Stats other) {
        this.manager = manager;
        PrepareValues();
        range.GetAndThen(v => v.Value.CStart());
        Inherit(other);
    }

    public void CStart(Manager manager, MTState state) {
        this.manager = manager;
        PrepareValues();
        mod = General.ListToDict(state.modkey, state.modVal);
        range.GetAndThen(v => v.Value.CStart());
        xp = state.xp;
        lvl = state.lvl;
        hp = state.hp;
    }

    public float modify(Stat value) {
        return mod[value]+incrs[value]*lvl;
    }

    public float get(Stat value) {
        return Num.MinLimit(val[value]*modify(value), 0.05f);
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

    private bool GetHit(float damage) {
        if (hp <= 0) { return false; }
        hp -= damage;
        if (hp <= 0) {
            Die(Option<Stats>.None());
            return true;
        }
        return false;
    }

    public void IncXP(int xp) {
        this.xp += xp;
        if (this.xp >= template.maxXp) {
            hp += Gameplay.MiniTowerLvlUpHPBonus*Num.MinLimit(lvl*0.25f, 1);
            lvl++;
            this.xp = 0;
        }
    }

    private Option<Stats> inheriter = Option<Stats>.None();
    public Option<Stats> Inheriter {
        get { return inheriter; }
    }
    public void Inherit(Stats other) {
        foreach (var k in mod.Keys.ToArray()) {
            mod[k] *= other.mod[k];
        }
        inheriter = new Option<Stats>(other);
    }

    public bool ImmuneTo(Element elm) {
        return StatElmImmunities.Contains(elm);
    }

    public void ConfigureOnDie(params OnDie[] calbacks) {
        onDie = calbacks[0];
        if (calbacks.Length == 1) {
            return;
        }
        for (int i = 1; i < calbacks.Length; i++) {
            onDie += calbacks[i];
        }
    }

    public void ConfigureOnStatDamage(params OnStatDamage[] calbacks) {
        onStatDamage = calbacks[0];
        if (calbacks.Length == 1) {
            return;
        }
        for (int i = 1; i < calbacks.Length; i++) {
            onStatDamage += calbacks[i];
        }
    }

    public void ConfigureOnBacteriaDamage(params OnBacteriaDamage[] calbacks) {
        onBacteriaDamage = calbacks[0];
        if (calbacks.Length == 1) {
            return;
        }
        for (int i = 1; i < calbacks.Length; i++) {
            onBacteriaDamage += calbacks[i];
        }
    }

    public void ConfigureOnExplosionDamage(params OnExplosionDamage[] calbacks) {
        onExplosionDamage = calbacks[0];
        if (calbacks.Length == 1) {
            return;
        }
        for (int i = 1; i < calbacks.Length; i++) {
            onExplosionDamage += calbacks[i];
        }
    }

    public void Die(Option<Stats> senderStats) {
        if (onDie != null)
            onDie.Invoke(senderStats);
        Destroy(gameObject);
    }

    public int Damage(Stats stats, float amount)
    {
        //Debug.Log("["+hp.Current+"] Being damaged: "+amount);
        //Debug.Log("from "+stats.gameObject.name);
        bool killed = GetHit(amount);
        if (killed && onStatDamage != null) {
            onStatDamage.Invoke(stats, amount);
        }
        return killed ? template.xpKillWorth : template.xpHitWorth;
    }

    public int BacteriaDamage(Element bt, float amount) {
        bool killed = GetHit(amount);
        if (killed && onBacteriaDamage != null) {
            onBacteriaDamage.Invoke(bt, amount);
        }
        return killed ? template.xpKillWorth : template.xpHitWorth;
    }

    public int ExplosionDamage(Explosion.Strength strength, float additive) {
        bool killed = GetHit(additive+(int)strength);
        if (killed && onExplosionDamage != null) {
            onExplosionDamage.Invoke(strength, additive);
        }
        return killed ? template.xpKillWorth : template.xpHitWorth;
    }
}
