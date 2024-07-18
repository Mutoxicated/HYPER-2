using System.Collections.Generic;
using System.Linq;
using Helpers;
using Unity.VisualScripting;
using UnityEngine;

public class World : MonoBehaviour
{
    [SerializeField] private GameObject tower;
    [SerializeField] private GameObject towerPlatform;
    [SerializeField] private WorldGenerator worldGenerator;
    
    private List<Enemy> enemies = new List<Enemy>();
    private List<MiniTower> mts = new List<MiniTower>();
    public delegate void ReportDeath<T>(T enemy) where T: ISideBelonger<T>;
    private ReportDeath<Enemy> reportEnemyDeath;
    private ReportDeath<MiniTower> reportPlayerDeath;

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

    public delegate void EnemyConstructor(ReportDeath<Enemy> death, Enemy enemy);

    public Enemy InstantiateEnemy(Resources.Enemy enemyType, Vector3 pos, EnemyConstructor es) {
        var enemy = Instantiate(Resources.GetEnemy(enemyType), pos, Quaternion.identity).GetComponent<Enemy>();
        enemies.Add(enemy);
        es(reportEnemyDeath, enemy);
        return enemy;
    }

    public delegate void MiniTowerConstructor(MiniTower mini);
    public MiniTower InstantiateMiniTower(Vector3 position, Resources.Arsenal arsenal, MiniTowerConstructor constructor) {
        var tower = Instantiate(Resources.GetArsenal(arsenal), position, Quaternion.identity).GetComponent<MiniTower>();
        constructor(tower);
        return tower;
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
        reportEnemyDeath = new ReportDeath<Enemy>(v => {
            enemies.Remove(v);
        });
        reportPlayerDeath = new ReportDeath<MiniTower>(v => {
            mts.Remove(v);
        });

        worldGenerator.DStart(this, player);
        
        foreach (var state in Save.rData.MTs) {
            RaycastHit info;
            Physics.Raycast(state.position, Vector3.down, out info, Mathf.Infinity);
            var mt = InstantiateMiniTower(state.position, state.arsenal, t => t.CStart(player.Manager, state));
            info.collider.transform.parent.GetComponent<Tile>().Occupy(mt.gameObject);
        }

        Save.rData.MTs.Clear();
        borderRadius = worldGenerator.GetLayerRadius();
        //Debug.Log(borderRadius);
    }
}
