using System.Collections.Generic;
using System.Linq;
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

    public WorldGenerator WG {
        get { return worldGenerator; }
    }

    public delegate void EnemyConstructor(ReportDeath death, Enemy enemy);

    public Enemy InstantiateEnemy(Resources.Enemy enemyType, Vector3 pos, EnemyConstructor es) {
        var enemy = Instantiate(Resources.GetEnemy(enemyType), pos, Quaternion.identity).GetComponent<Enemy>();
        enemies.Add(enemy);
        es(reportDeath, enemy);
        return enemy;
    }

    private Dictionary<int,float> info = new Dictionary<int,float>();

    public List<Enemy> GetClosestEnemies(Transform transform, float maxDist) {
        var list = enemies.ToList();

        for (int i = 0; i < list.Count; i++) {
            if (i == 0) {
                continue;
            }

            for (int j = i-1; j >= 0; j--) {
                var containsE1 = info.ContainsKey(list[i].GetInstanceID());
                var containsE2 = info.ContainsKey(list[j].GetInstanceID());
                
                var dist1 = containsE1 ? info[list[i].GetInstanceID()] : Vector3.Distance(list[i].transform.position, transform.position);
                var dist2 = containsE2 ? info[list[j].GetInstanceID()] : Vector3.Distance(list[j].transform.position, transform.position);

                if (dist1 > maxDist) {
                    list.Remove(list[i]);
                    i--;
                    continue;
                }
                if (dist2 > maxDist) {
                    list.Remove(list[j]);
                    i--;
                    continue;
                }

                if (!containsE1) {
                    info.Add(list[i].GetInstanceID(), dist1);
                }

                if (!containsE2) {
                    info.Add(list[j].GetInstanceID(), dist2);
                }
                
                if (dist2 < dist1) {
                    var temp = list[i];
                    list[i] = list[j];
                    list[j] = temp;
                }
            }
        }

        info.Clear();

        return list;
    }

    public void DStart(Player player) {
        reportDeath = new ReportDeath(e => enemies.Remove(e));
        worldGenerator.DStart(this, player);
        borderRadius = worldGenerator.GetLayerRadius();
        //Debug.Log(borderRadius);
    }
}
