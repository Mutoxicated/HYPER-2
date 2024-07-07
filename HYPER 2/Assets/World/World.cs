using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class World : MonoBehaviour
{
    [SerializeField] private GameObject tower;
    [SerializeField] private GameObject towerPlatform;
    [SerializeField] private WorldGenerator worldGenerator;
    
    private List<Enemy> enemies = new List<Enemy>();
    public delegate void ReportDeath(Enemy enemy);
    private ReportDeath reportDeath;

    private float borderRadius;
    public float BorderRadius {
        get { return borderRadius; }
    }

    public GameObject Tower {
        get { return tower; }
    }

     public GameObject TowerPlatform {
        get { return towerPlatform; }
    }

    public delegate void EnemyConstructor(ReportDeath death, Enemy enemy);

    public Enemy InstantiateEnemy(Resources.Enemy enemyType, Vector3 pos, EnemyConstructor es) {
        var enemy = Instantiate(Resources.GetEnemy(enemyType), pos, Quaternion.identity).GetComponent<Enemy>();
        enemies.Add(enemy);
        es(reportDeath, enemy);
        return enemy;
    }

    public void DStart(Player player) {
        reportDeath = new ReportDeath(e => enemies.Remove(e));
        worldGenerator.DStart(this, player);
        borderRadius = worldGenerator.GetLayerRadius();
        //Debug.Log(borderRadius);
    }
}
