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
        ENEMY,
        NEUTRAL
    }

    [SerializeField] private Option<GameObject> holder = Option<GameObject>.None();
    [SerializeField] private StatsTemplate template;
    [SerializeField] private Option<HitVisual> hitVisual = Option<HitVisual>.None();
    [SerializeField] private Option<Range> range;

    private Dictionary<Stat, float> mod = new Dictionary<Stat, float>();
    private Dictionary<Stat, float> val = new Dictionary<Stat, float>();
    private readonly Dictionary<Stat, float> incrs = new Dictionary<Stat, float>();

    public float this[Stat stat] {
        get {
            return get(stat);
        }
    }

    private float hp;
    private int xp = 0;
    private int lvl = 1;

    private Manager manager; // IMMUTABLE
    public delegate void OnLvl();
    public delegate void OnDie(Option<Stats> stats);
    public delegate int OnStatDamage(Stats stats, float amount);
    public delegate int OnBacteriaDamage(Element elm, float amount);
    public delegate int OnExplosionDamage(Explosion.Strength strength, float additive);

    public OnDie onDie;
    public OnStatDamage onStatDamage;
    public OnBacteriaDamage onBacteriaDamage;
    public OnExplosionDamage onExplosionDamage;
    public OnLvl onLvlUp;

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

    public Option<Stats> Target(int offset = 0) {
        return range.Get().Target(offset);
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

    public Option<Range> Range {
        get { return range; }
    }

    public void RestoreHP() {
        hp = get(Stat.MAX_HEALTH);
    }

    private void PrepareValues() {
        onDie = new OnDie(s => {Destroy(holder.Get());});
        onBacteriaDamage = new OnBacteriaDamage((elm, amount) => {
            bool killed = GetHit(amount);
            int xp = killed ? template.xpKillWorth : 0;
            if (template.statsBase.side == Side.ENEMY  & xp > 0)
                manager.GUI.PopupPool.Popup(transform.position+Vector3.up, Color.cyan, "+"+xp);
            if (hitVisual.IsSome())
                hitVisual.Get().OnHit(0.5f);
            return xp;
        });
        onExplosionDamage = new OnExplosionDamage((strength, amount) => {
            bool killed = GetHit((amount+(int)strength)*get(Stat.ARMOR));
            int xp = killed ? template.xpKillWorth : 0;
            if (template.statsBase.side == Side.ENEMY  & xp > 0)
                manager.GUI.PopupPool.Popup(transform.position+Vector3.up, Color.cyan, "+"+xp);
            if (hitVisual.IsSome()) 
                hitVisual.Get().OnHit(1f);
            return xp;
        });
        onStatDamage = new OnStatDamage((stats, amount) => {
            bool killed = GetHit(amount*get(Stat.ARMOR));
            int xp = killed ? template.xpKillWorth : 0;
            if (template.statsBase.side == Side.ENEMY & xp > 0)
                manager.GUI.PopupPool.Popup(transform.position+Vector3.up, Color.cyan, "+"+xp);
            if (hitVisual.IsSome()) 
                hitVisual.Get().OnHit(0.75f);
            return xp;
        });
        if (holder.IsNone()) {
            holder = new Option<GameObject>(gameObject);
        }
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
        range.GetAndThen(v => v.CStart());
    }

    public void CStart(Manager manager, Stats other) {
        this.manager = manager;
        PrepareValues();
        range.GetAndThen(v => v.CStart());
        Inherit(other);
    }

    public void CStart(Manager manager, MTState state) {
        this.manager = manager;
        PrepareValues();
        mod = General.ListToDict(state.modkey, state.modVal);
        range.GetAndThen(v => v.CStart());
        xp = state.xp;
        lvl = state.lvl;
        hp = state.hp;
    }

    public float modify(Stat value) {
        return mod[value]+incrs[value]*lvl;
    }

    private float get(Stat value) {
        return Num.MinLimit(val[value]*modify(value), 0.05f);
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

    public bool GetHit(float damage) {
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
            Heal(Gameplay.MiniTowerLvlUpHPBonus*Num.MinLimit(lvl*0.25f, 1));
            lvl++;
            if (onLvlUp != null)
                onLvlUp.Invoke();
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

    public void Heal(float amount) {
        hp = Mathf.Clamp(hp+amount, 0, get(Stat.MAX_HEALTH));
    }

    public bool ImmuneTo(Element elm) {
        return StatElmImmunities.Contains(elm);
    }

    public void ConfigureOnDie(bool reset = false, params OnDie[] calbacks) {
        if (reset)
            onDie = new OnDie(calbacks[0]);
        else onDie += calbacks[0];
        if (calbacks.Length == 1) {
            return;
        }
        for (int i = 1; i < calbacks.Length; i++) {
            onDie += calbacks[i];
        }
    }

    public void ConfigureOnStatDamage(bool reset = false, params OnStatDamage[] calbacks) {
        if (reset)
            onStatDamage = new OnStatDamage(calbacks[0]);
        else onStatDamage += calbacks[0];
        if (calbacks.Length == 1) {
            return;
        }
        for (int i = 1; i < calbacks.Length; i++) {
            onStatDamage += calbacks[i];
        }
    }

    public void ConfigureOnBacteriaDamage(bool reset = false, params OnBacteriaDamage[] calbacks) {
        if (reset)
            onBacteriaDamage = new OnBacteriaDamage(calbacks[0]);
        else onBacteriaDamage += calbacks[0];
        if (calbacks.Length == 1) {
            return;
        }
        for (int i = 1; i < calbacks.Length; i++) {
            onBacteriaDamage += calbacks[i];
        }
    }

    public void ConfigureOnExplosionDamage(bool reset = false, params OnExplosionDamage[] calbacks) {
        if (reset)
            onExplosionDamage = new OnExplosionDamage(calbacks[0]);
        else onExplosionDamage += calbacks[0];
        if (calbacks.Length == 1) {
            return;
        }
        for (int i = 1; i < calbacks.Length; i++) {
            onExplosionDamage += calbacks[i];
        }
    }

    public void Die(Option<Stats> senderStats) {
        onDie.Invoke(senderStats);
    }

    public int Damage(Stats stats, float amount)
    {
        var delegates = onStatDamage.GetInvocationList();
        if (delegates.Length == 1) {
            return onStatDamage.Invoke(stats, amount);
        }
        var ret = delegates[0].DynamicInvoke(stats, amount);

        for (int i = 1; i < delegates.Length; i++) {
            delegates[i].DynamicInvoke(stats, amount);
        }
    
        return (int)ret;
    }

    public int BacteriaDamage(Element bt, float amount) {
        var delegates = onBacteriaDamage.GetInvocationList();
        if (delegates.Length == 1) {
            return onBacteriaDamage.Invoke(bt, amount);
        }
        var ret = delegates[0].DynamicInvoke(bt, amount);

        for (int i = 1; i < delegates.Length; i++) {
            delegates[i].DynamicInvoke();
        }
    
        return (int)ret;
    }

    public int ExplosionDamage(Explosion.Strength strength, float additive) {
        var delegates = onExplosionDamage.GetInvocationList();
        if (delegates.Length == 1) {
            return onExplosionDamage.Invoke(strength, additive);
        }
        var ret = delegates[0].DynamicInvoke(strength, additive);

        for (int i = 1; i < delegates.Length; i++) {
            delegates[i].DynamicInvoke();
        }
    
        return (int)ret;
    }
}
