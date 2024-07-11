using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Stat;

public class Range : MonoBehaviour
{
    [SerializeField] private Stats stats;

    private List<Enemy> enemies = new List<Enemy>();

    public void CStart() {
        var tileSize = stats.Manager.World.WG.TileSize;
        transform.localScale = new Vector3(stats.get(RANGE)*tileSize*4, transform.localScale.y, stats.get(RANGE)*tileSize*4);
        gameObject.SetActive(true);
    }

    public void OnTriggerEnter(Collider collider) {
        var enemy = collider.gameObject.GetComponent<Enemy>();
        //enemy.ReportDeath(e => enemies.Remove(e));
        enemies.Add(enemy);
    }

    public void OnTriggerExit(Collider collider) {
        var enemy = collider.gameObject.GetComponent<Enemy>();
        enemies.Remove(enemy);
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
