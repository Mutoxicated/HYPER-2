using System.Collections;
using System.Collections.Generic;
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
        var tileSize = stats.Manager.World.WG.TileSize;
        transform.localScale = new Vector3(stats.get(RANGE)*tileSize*4-tileSize, transform.localScale.y, stats.get(RANGE)*tileSize*4-tileSize);
    }

    public void CStart() {
        stats.onLvlUp = UpdateSize;
        OnEnter = new OnEnemy(e => enemies.Add(e));
        OnExit = new OnEnemy(e => enemies.Remove(e));
        UpdateSize();
        gameObject.SetActive(true);
    }

    public void OnTriggerEnter(Collider collider) {
        var enemy = collider.gameObject.GetComponent<Stats>();
        if (enemy == null) {
            return;
        }else if (enemy == stats) {
            return;
        }
        OnEnter.Invoke(enemy);
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
            return Target();
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
