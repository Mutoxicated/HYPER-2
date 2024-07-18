using System.Collections.Generic;
using UnityEngine;

public class Gameplay : MonoBehaviour
{
    private static Gameplay self;

    // Difficulty
    private float linearT = 1f;
    public float LinearT {
        get { return linearT; }
    }
    public float AsymT {
        get { return Mathf.Sqrt(linearT); }
    }
    public int IntAsymT {
        get { return Mathf.CeilToInt(Mathf.Sqrt(linearT)); }
    }

    // Settings
    public int enemiesAmount = 10;
    

    // Unlocks
    public const int unlock2ndPath = 5;
    public const int unlock3rdPath = 10;
    public const int unlock4thPath = 20;

    [SerializeField] private QuantitativeRound quantitativeRound;
    [SerializeField] private Interval waitInterval;

    private Dictionary<int, List<Resources.Enemy>> enemyDifficulties = new Dictionary<int, List<Resources.Enemy>>();

    // Dependencies
    private Manager manager;
    private World world;
    public World World {
        get { return world; }
    }

    public void DStart(Player player, Manager manager) {
        self = this;
        this.manager = manager;
        world = manager.World;

        player.AddLvlUpListener(lvl => {
            switch (lvl) {
                case unlock2ndPath:
                    world.WG.CreateNextPath();
                    break;
                case unlock3rdPath:
                    world.WG.CreateNextPath();
                    break;
                case unlock4thPath:
                    world.WG.CreateNextPath();
                    break;
            }
        });
        player.onLvlUp.Invoke(Save.rData.Lvl);
        
        var iter = Resources.IterEnemyStats();
        using (iter) {
            while (iter.MoveNext()) {
                if (!enemyDifficulties.ContainsKey(iter.Current.Value.difficulty)) {
                    enemyDifficulties.Add(iter.Current.Value.difficulty, new List<Resources.Enemy>() {
                        iter.Current.Key
                    });
                }else {
                    enemyDifficulties[iter.Current.Value.difficulty].Add(iter.Current.Key);
                }
            }
        }

        waitInterval.Enable();
    }

    public Enemy SpawnEnemy(Vector3 pos, Branch root) {
        var diff = IntAsymT;
        return world.InstantiateEnemy(enemyDifficulties[diff][Seed.IntRange(0, enemyDifficulties[diff].Count)], pos, (r, e) => {
            e.CStart(manager, r, root);
        });
    }

    public void IntervalStartWave() {
        Save.rData.IncrementWaves();
        quantitativeRound.Enable();
        waitInterval.Disable();
    }

    public void Update() {
        linearT += Time.deltaTime/30f;
    }

    public void EndWave() {
        waitInterval.Enable();
    }
}
