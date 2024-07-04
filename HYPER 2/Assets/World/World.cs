using Unity.VisualScripting;
using UnityEngine;

public class World : MonoBehaviour
{
    [SerializeField] private GameObject tower;
    [SerializeField] private GameObject towerPlatform;
    [SerializeField] private WorldGenerator worldGenerator;
    [SerializeField] private GameObject[] enemyPrefabs;

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

    public delegate void EnemyConstructor(Enemy enemy);

    public Enemy InstantiateEnemy(int index, Vector3 pos, EnemyConstructor es) {
        var enemy = Instantiate(enemyPrefabs[index], pos, Quaternion.identity).GetComponent<Enemy>();
        es(enemy);
        return enemy;
    }

    public void DStart(Player player) {
        worldGenerator.DStart(this, player);
        borderRadius = worldGenerator.GetLayerRadius();
        //Debug.Log(borderRadius);
    }
}
