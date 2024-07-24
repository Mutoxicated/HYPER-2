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
    public const int unlock1stWidePath = 30;
    public const int unlock2ndWidePath = 40;
    public const int unlock3rdWidePath = 50;
    public const int unlock4ndWidePath = 60;

    [SerializeField] private QuantitativeRound quantitativeRound;
    [SerializeField] private Interval waitInterval;

    private Dictionary<int, List<Resources.Enemy>> enemyDifficulties = new Dictionary<int, List<Resources.Enemy>>();

    // Dependencies
    private Manager manager;
    private World world;
    public World World {
        get { return world; }
    }

    private void UnlockLogic() {
        var lvl = Save.rData.Lvl;

        if (lvl >= unlock2ndPath) {
            world.WG.CreateNextPath();
        }
        if (lvl >= unlock3rdPath) {
            world.WG.CreateNextPath();
        }
        if (lvl >= unlock4thPath) {
            world.WG.CreateNextPath();
        }
        if (lvl >= unlock1stWidePath) {
            world.WG.WidenNextPath();
        }
        if (lvl >= unlock2ndWidePath) {
            world.WG.WidenNextPath();
        }
        if (lvl >= unlock3rdWidePath) {
            world.WG.WidenNextPath();
        }
        if (lvl >= unlock4ndWidePath) {
            world.WG.WidenNextPath();
        }
    }

    public void DStart(Player player, Manager manager) {
        self = this;
        this.manager = manager;
        world = manager.World;

        UnlockLogic();

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
                case unlock1stWidePath:
                    world.WG.WidenNextPath();
                    break;
                case unlock2ndWidePath:
                    world.WG.WidenNextPath();
                    break;
                case unlock3rdWidePath:
                    world.WG.WidenNextPath();
                    break;
                case unlock4ndWidePath:
                    world.WG.WidenNextPath();
                    break;
            }
        });
        
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

    public void EndWave() {
        linearT += Time.deltaTime/10f;
        waitInterval.Enable();
    }
}
