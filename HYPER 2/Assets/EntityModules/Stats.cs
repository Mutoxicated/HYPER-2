using System.Collections.Generic;
using Helpers;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public enum Element {
    WATER,
    FIRE,
    RADIATION,
    AIR,
    EARTH,
    ELECTRICITY
}

public class TargetMessage {
    private readonly int value = 0;
    
    private const int val = 2;

    public static readonly TargetMessage OUT_OF_BOUNDS = new TargetMessage(Type.OUT_OF_BOUNDS);
    public static readonly TargetMessage INVALID = new TargetMessage(Type.INVALID);
    public static readonly TargetMessage VALID = new TargetMessage(Type.VALID);

    public enum Type {
        OUT_OF_BOUNDS,
        INVALID,
        VALID
    }

    public TargetMessage(Type v) {
        value = (int)v;
    }

    public static implicit operator bool(TargetMessage msg) {
        return msg.value == val;
    }

    public override bool Equals(object msg) {
        return value.Equals(((TargetMessage)msg).value);
    }
    
    public override int GetHashCode()
    {
        return value.GetHashCode();
    }
}

public interface IStats : IDamageable {
    Stats Convert();
}

public class Stats : MonoBehaviour, IStats
{
    public enum Side {
        PLAYER,
        ENEMY,
        NEUTRAL
    }

    [SerializeField] private Option<GameObject> holder = Option<GameObject>.None();
    [SerializeField] private float height = 4f;
    [SerializeField] private Option<HitVisual> hitVisual = Option<HitVisual>.None();
    [SerializeField] private Option<Range> range;
    [SerializeField] private Option<Infector> infector;

    private StatsTemplate template;
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
    public delegate void OnStatChange();

    public OnDie onDie;
    public OnDie onBeforeDeath;
    public OnStatDamage onStatDamage;
    public OnBacteriaDamage onBacteriaDamage;
    public OnExplosionDamage onExplosionDamage;
    public OnLvl onLvlUp;
    public Dictionary<Stat, OnStatChange> statChangeCallbacks = new Dictionary<Stat, OnStatChange>();

    public Option<GameObject> Holder {
        get { return holder; }
    }

    public Dictionary<Stat, float> Mod {
        get { return mod; }
    }

    public float HP {
        get { return hp; }
    }

    public int XP
    {
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
    
    public Option<Stats> Target(int offset = 0)
    {
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

    public Option<Infector> Infector {
        get { return infector; }
    }

    public Option<Range> Range {
        get { return range; }
    }

    public void RestoreHP() {
        hp = get(Stat.MAX_HEALTH);
    }

    private void LvlUpPopup() {
        manager.GUI.PopupPool.Popup(
            position: transform.position+Vector3.up*height, 
            color: Color.cyan,
            text: "Lvl Up!",
            duration: 4,
            dy: 0.3f
        );
    }

    private void PrepareValues() {
        onDie = new OnDie(_ => Destroy(holder.Get()));
        onLvlUp = new OnLvl(() => {
            Heal(Gameplay.MiniTowerLvlUpHPBonus*Num.MinLimit(lvl*0.25f, 1));
        });
        onBacteriaDamage = new OnBacteriaDamage((elm, amount) => {
            bool killed = GetHit(Option<Stats>.None(), amount);
            int xp = killed ? template.xpKillWorth/2 : 0;
            if (template.statsBase.side == Side.ENEMY  & xp > 0)
                manager.GUI.PopupPool.Popup(transform.position+Vector3.up, Color.cyan, "+"+xp);
            if (hitVisual.IsSome())
                hitVisual.Get().OnHit(0.5f);
            return xp;
        });
        onExplosionDamage = new OnExplosionDamage((strength, amount) => {
            bool killed = GetHit(Option<Stats>.None(), (amount+(int)strength)*get(Stat.ARMOR));
            int xp = killed ? template.xpKillWorth : 0;
            if (template.statsBase.side == Side.ENEMY  & xp > 0)
                manager.GUI.PopupPool.Popup(transform.position+Vector3.up, Color.cyan, "+"+xp);
            if (hitVisual.IsSome()) 
                hitVisual.Get().OnHit(1f);
            return xp;
        });
        onStatDamage = new OnStatDamage((stats, amount) => {
            bool killed = GetHit(new Option<Stats>(stats), amount*get(Stat.ARMOR));
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

    public void CStart(Manager manager, StatsTemplate template) {
        this.template = template;
        this.manager = manager;
        PrepareValues();
        range.GetAndThen(v => v.CStart());
    }

    public void CStart(Manager manager, StatsTemplate template, Stats other) {
        this.template = template;
        this.manager = manager;
        PrepareValues();
        range.GetAndThen(v => v.CStart());
        Inherit(other);
    }

    public void CStart(Manager manager, StatsTemplate template, MTState state) {
        this.template = template;
        this.manager = manager;
        PrepareValues();
        range.GetAndThen(v => v.CStart());
        xp = state.xp;
        lvl = state.lvl;
        hp = state.hp;
    }

    private float incr;
    public float modify2(Stat value) {
        incr = incrs[value]*(lvl-1);
        return mod[value]-Mathf.InverseLerp(1, 0, incr)*incr;
    }

    public float modify(Stat value) {
        return mod[value]+incrs[value]*(lvl-1);
    }

    private float get(Stat value) {
        return val[value]*Num.MinLimit(modify(value), 0.05f);
    }

    /// <summary>
    /// Only call this on RATE, ARMOR and IMMUNITY
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public float get2(Stat value) {
        return val[value]*Num.MinLimit(modify2(value), 0.05f);
    }

    public void inc(Stat stat, float value) {
        mod[stat] += value;
        if (statChangeCallbacks.ContainsKey(stat)) {
            statChangeCallbacks[stat]();
        }
    }

    public TargetMessage TargetIsValid(int offset = 0) {
        if (range.IsNone()) {
            return new TargetMessage(TargetMessage.Type.INVALID);
        }
        return range.Get().TryTarget(offset);
    }

    public bool GetHit(Option<Stats> stats, float damage) {
        if (hp <= 0) { return false; }
        hp -= damage;
        if (hp <= 0) {
            Die(stats);
            return true;
        }
        return false;
    }

    public void LvlUp(bool restoreCompletely = false, bool showPopup = true) {
        if (onLvlUp != null)
            onLvlUp.Invoke();
        if (showPopup & StatSide == Side.PLAYER & gameObject.name != "Tower")
            LvlUpPopup();
        lvl++;
    }

    public int MaxRequiredXP() {
        return template.maxXp+template.maxXpIncrememt*(lvl-1);
    }

    public void IncXP(int xp, bool showPopup = true) {
        this.xp += xp;
        while (this.xp >= MaxRequiredXP()) {
            this.xp -= MaxRequiredXP();
            LvlUp(showPopup: showPopup);
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
        infector.GetAndThen(i => {
            other.Infector.GetAndThen(i2 => {
                i.Inherit(i2);
            });
        });
        inheriter = new Option<Stats>(other);
    }

    public bool ImmuneTo(Element elm) {
        return StatElmImmunities.Contains(elm);
    }

    public void AddStatChangeCallback(Stat stat, OnStatChange cb) {
        if (statChangeCallbacks.ContainsKey(stat)) {
            statChangeCallbacks[stat] += cb;
        }else {
            statChangeCallbacks.Add(stat, cb);
        }
    }

    public void ConfigureOnLvlUp(bool reset = false, params OnLvl[] calbacks) {
        if (reset)
            onLvlUp = new OnLvl(calbacks[0]);
        else {
            if (onLvlUp == null) {
                onLvlUp = new OnLvl(calbacks[0]);
            }else {
                onLvlUp += calbacks[0];
            }
        }
        if (calbacks.Length == 1) {
            return;
        }
        for (int i = 1; i < calbacks.Length; i++) {
            onLvlUp += calbacks[i];
        }
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

    public void ConfigureOnBeforeDeath(bool reset = false, params OnDie[] calbacks) {
        if (reset)
            onBeforeDeath = new OnDie(calbacks[0]);
        else {
            if (onBeforeDeath == null) {
                onBeforeDeath = new OnDie(calbacks[0]);
            }else {
                onBeforeDeath += calbacks[0];
            }
        }
        if (calbacks.Length == 1) {
            return;
        }
        for (int i = 1; i < calbacks.Length; i++) {
            onBeforeDeath += calbacks[i];
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
        if (onBeforeDeath != null)
            onBeforeDeath.Invoke(senderStats);
        onDie.Invoke(senderStats);
    }

    public void Heal(float amount, bool show = false) {
        if (show)
            manager.GUI.PopupPool.Popup(
                 position: transform.position+Vector3.up*height, 
            color: Color.green,
            text: "+"+amount,
            duration: 1,
            dy: 0.5f
        );
        hp = Mathf.Clamp(hp+amount, 0, get(Stat.MAX_HEALTH));
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
            return onBacteriaDamage.Invoke(bt, amount*this[Stat.IMMUNITY]);
        }
        var ret = delegates[0].DynamicInvoke(bt, amount*this[Stat.IMMUNITY]);

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

    public Stats Convert()
    {
        return this;
    }
}
