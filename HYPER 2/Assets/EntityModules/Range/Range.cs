using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;
using static Stat;

public interface ISideBelonger<T> where T: ISideBelonger<T>{
    Stats.Side Side();
    GameObject Go();
    bool IsNull();
    ref World.ReportDeath<T> GetReportDeath();
}

public delegate bool Predicate<E>(E t);
public delegate void Iter<E>(E t);

public class Range : MonoBehaviour
{
    [SerializeField] private Stats stats;

    private List<Stats> enemies = new List<Stats>();

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
        stats.ConfigureOnLvlUp(false, UpdateSize);
        OnEnter = new OnEnemy(e => enemies.Add(e));
        OnExit = new OnEnemy(e => enemies.Remove(e));
        UpdateSize();
        gameObject.SetActive(true);
    }

    public void OnTriggerEnter(Collider collider) {
        var enemy = collider.gameObject.GetComponent<IStats>();
        if (enemy == null) {
            return;
        }else if (enemy.Convert() == stats) {
            return;
        }
        OnEnter.Invoke(enemy.Convert());
    }

    public void OnTriggerExit(Collider collider) {
        var enemy = collider.gameObject.GetComponent<Stats>();
        OnExit.Invoke(enemy);
    }

    public Option<Stats> Target(int offset = 0) {
        if (enemies.Count < offset+1) {
            return Option<Stats>.None();
        }
        if (enemies[offset] == null) {
            enemies.RemoveAt(offset);
            return Target(Num.MinLimit(offset-1, 0));
        }
        if ((enemies[offset].StatSide == Stats.Side.PLAYER & stats.StatSide == Stats.Side.PLAYER) |
            (enemies[offset].StatSide == Stats.Side.ENEMY & stats.StatSide == Stats.Side.ENEMY)) {
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
