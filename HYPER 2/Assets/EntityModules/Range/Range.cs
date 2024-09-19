using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;
using static Stat;
using static TargetMessage;

public delegate bool Predicate<E>(E t);
public delegate void Iter<E>(E t);

public class Range : MonoBehaviour
{
    [SerializeField] private Stats stats;
    [SerializeField] private bool expand = true;
    [SerializeField] private Stats.Side filter;

    public List<Stats> enemies = new List<Stats>();

    public List<Stats> Enemies {
        get { return enemies; }
    }

    public delegate void OnEnemy(Stats e);

    public OnEnemy OnEnter;
    public OnEnemy OnExit;

    private void UpdateSize() {
        var rangeSize = stats.Manager.World.WG.RangeSize(stats[RANGE]);
        transform.localScale = new Vector3(rangeSize, transform.localScale.y, rangeSize);
    }

    public void CStart() {
        OnEnter = new OnEnemy(e => enemies.Add(e));
        OnExit = new OnEnemy(e => enemies.Remove(e));
        stats.AddStatChangeCallback(RANGE, () => {
            UpdateSize();
        });
        if (expand){
            stats.ConfigureOnLvlUp(false, UpdateSize);
            UpdateSize();
        }
        gameObject.SetActive(true);
    }

    public void OnTriggerEnter(Collider collider) {
        var enemy = collider.gameObject.GetComponent<IStats>();
        if (enemy == null) {
            return;
        }
        if (enemy.Convert() == null) {
            return;
        }
        if (enemy.Convert() == stats) {
            return;
        }
        if (Filter(enemy.Convert()).IsNone()) {
            return;
        }
        OnEnter.Invoke(enemy.Convert());
    }

    public void OnTriggerExit(Collider collider) {
        var enemy = collider.gameObject.GetComponent<IStats>();
        OnExit.Invoke(enemy.Convert());
    }

    public TargetMessage TryTarget(int offset = 0) {
        if (enemies.Count < offset+1) {
            return OUT_OF_BOUNDS;
        }
        if (enemies[offset] == null || enemies[offset].gameObject == null) {
            enemies.RemoveAt(offset);
            return TryTarget(offset);
        }
        return VALID;
    }

    public Option<Stats> Filter(Stats s) {
        if (s.StatSide == filter) {
            return Option<Stats>.None();
        }
        return new Option<Stats>(s);
    }

    public Option<Stats> Target(int offset = 0) {
        if (enemies.Count < offset+1) {
            return Option<Stats>.None();
        }
        if (enemies[offset] == null || enemies[offset].gameObject == null) {
            enemies.RemoveAt(offset);
            return Target(offset);
        }
        if (enemies[offset].StatSide == filter) {
            return Target(offset+1);
        }
        return new Option<Stats>(enemies[offset]);
    }

    /// <summary>
    /// Tries to find a gameObject that matches the predicate, otherwise falls back to choosing the first element in the enemies list.
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public Option<Stats> Find(Predicate<Stats> go) {
        if (enemies.Count == 0) {
            return Option<Stats>.None();
        }
        foreach (var target in enemies.ToArray()) {
            if (target == null) {
                Enemies.Remove(target);
            }

            if (go(target)) {
                return new Option<Stats>(target);
            }
        }
        return Target();
    }

    public void Foreach(Iter<Stats> iter) {
        foreach (var enemy in enemies) {
            if (enemy == null) {
                continue;
            }
            iter(enemy);
        }
    }
}
