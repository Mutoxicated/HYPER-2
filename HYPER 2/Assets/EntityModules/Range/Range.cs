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
public delegate void Iter(GameObject go);

public interface IRange {
    public void CStart();
    public Option<GameObject> Target(int offset = 0);
    public Option<GameObject> Find(Predicate<GameObject> go);
    public void Foreach(Iter iter);
}

public class Range<T> : MonoBehaviour, IRange where T: ISideBelonger<T>
{
    [SerializeField] private Stats stats;

    private List<T> enemies = new List<T>();

    public List<T> Enemies {
        get { return enemies; }
    }

    public delegate void OnEnemy(T e);

    public OnEnemy OnEnter;
    public OnEnemy OnExit;

    public void CStart() {
        OnEnter = new OnEnemy(e => enemies.Add(e));
        OnExit = new OnEnemy(e => enemies.Remove(e));
        var tileSize = stats.Manager.World.WG.TileSize;
        transform.localScale = new Vector3(stats.get(RANGE)*tileSize*4-tileSize, transform.localScale.y, stats.get(RANGE)*tileSize*4-tileSize);
        gameObject.SetActive(true);
    }

    public void OnTriggerEnter(Collider collider) {
        var enemy = collider.gameObject.GetComponent<T>();
        if (enemy == null) {
            return;
        }
        enemy.GetReportDeath() += e => OnExit.Invoke(e);
        OnEnter.Invoke(enemy);
    }

    public void OnTriggerExit(Collider collider) {
        var enemy = collider.gameObject.GetComponent<T>();
        OnExit.Invoke(enemy);
    }

    public Option<GameObject> Target(int offset = 0) {
        if (enemies.Count < offset+1) {
            return Option<GameObject>.None();
        }
        if (enemies[offset].IsNull()) {
            enemies.RemoveAt(offset);
            return Target();
        }
        return new Option<GameObject>(enemies[offset].Go());
    }


    /// <summary>
    /// Tries to find a gameObject that matches the predicate, otherwise falls back to choosing the first element in the enemies list.
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public Option<GameObject> Find(Predicate<GameObject> go) {
        if (enemies.Count == 0) {
            return Option<GameObject>.None();
        }
        foreach (var target in enemies.ToArray()) {
            if (target == null) {
                Enemies.Remove(target);
            }

            if (go(target.Go())) {
                return new Option<GameObject>(target.Go());
            }
        }
        return Target();
    }

    public void Foreach(Iter iter) {
        foreach (var enemy in enemies) {
            if (enemy == null) {
                continue;
            }
            iter(enemy.Go());
        }
    }
}
