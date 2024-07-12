using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Stat;

public class Range : MonoBehaviour
{
    [SerializeField] private Stats stats;

    private List<Enemy> enemies = new List<Enemy>();

    public List<Enemy> Enemies {
        get { return enemies; }
    }

    public delegate void OnEnemy(Enemy e);

    public OnEnemy OnEnter;
    public OnEnemy OnExit;

    public void CStart() {
        OnEnter = new OnEnemy(e => enemies.Add(e));
        OnExit = new OnEnemy(e => enemies.Remove(e));
        var tileSize = stats.Manager.World.WG.TileSize;
        transform.localScale = new Vector3(stats.get(RANGE)*tileSize*4, transform.localScale.y, stats.get(RANGE)*tileSize*4);
        gameObject.SetActive(true);
    }

    public void OnTriggerEnter(Collider collider) {
        var enemy = collider.gameObject.GetComponent<Enemy>();
        enemy.ReportDeath(e => OnExit.Invoke(e));
        OnEnter.Invoke(enemy);
    }

    public void OnTriggerExit(Collider collider) {
        var enemy = collider.gameObject.GetComponent<Enemy>();
        OnExit.Invoke(enemy);
    }

    public Option<GameObject> Target() {
        if (enemies.Count == 0) {
            return Option<GameObject>.None();
        }
        if (enemies[0] == null) {
            enemies.RemoveAt(0);
            return Target();
        }
        return new Option<GameObject>(enemies[0].gameObject);
    }

}
