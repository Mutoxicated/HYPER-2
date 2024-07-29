using System.Collections.Generic;
using UnityEngine;

public class Gameplay : MonoBehaviour
{
    private static Gameplay self;

    // Difficulty
    public int Waves {
        get { return Save.rData.Waves; }
    }
    public float LinearT {
        get { return Save.rData.LinearTime; }
    }
    public int IntLinearT {
        get { return Mathf.RoundToInt(LinearT); }
    }

    public float AsymT {
        get { return Mathf.Sqrt(LinearT); }
    }
    public int IntAsymT {
        get { return Mathf.CeilToInt(Mathf.Sqrt(LinearT)); }
    }

    // Settings
    private int enemiesAmount = 3;
    public int EnemiesAmount {
        get { return Mathf.RoundToInt(enemiesAmount*LinearT); }
    }

    public const int MiniTowerLvlUpHPBonus = 10;

    // Unlocks
    public const int unlock2ndPath = 5;
    public const int unlock3rdPath = 10;
    public const int unlock4thPath = 20;
    public const int unlock1stWidePath = 30;
    public const int unlock2ndWidePath = 40;
    public const int unlock3rdWidePath = 50;
    public const int unlock4thWidePath = 60;

    [SerializeField] private QuantitativeRound quantitativeRound;
    [SerializeField] private Interval waitInterval;

    private Dictionary<int, List<Resources.Enemy>> enemyDifficulties = new Dictionary<int, List<Resources.Enemy>>();

    // Dependencies
    private Manager manager;
    private World world;
    public World World {
        get { return world; }
    }

    public void GenerationUnlockLogic() {
        var lvl = Save.rData.Lvl;

        if (lvl >= unlock2ndPath) {
            world.WG.CreateNextPath();
        }
        if (lvl >= unlock3rdPath) {
            world.WG.CreateNextPath();
        }
        if (lvl >= unlock4thPath) {
            world.WG.CreateNextPath();
            world.WG.NewGoldPile();
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
        if (lvl >= unlock4thWidePath) {
            world.WG.WidenNextPath();
        }

        if (Save.rData.Waves >= 25) {
            var expansionsOwed = Mathf.FloorToInt(Save.rData.Waves % 25);
            for (int i = 1; i <= expansionsOwed; i++) {
                world.WG.Expand(5);
            }
        }

        if (Waves >= 50) {
            var goldPilesOwed = Mathf.FloorToInt(Waves % 50); 
            for (int i = 1; i < goldPilesOwed; i++) {
                world.WG.NewGoldPile();
            }
        }
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
                case unlock1stWidePath:
                    world.WG.WidenNextPath();
                    break;
                case unlock2ndWidePath:
                    world.WG.WidenNextPath();
                    break;
                case unlock3rdWidePath:
                    world.WG.WidenNextPath();
                    break;
                case unlock4thWidePath:
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
        var diff = IntLinearT;
        return world.InstantiateEnemy(enemyDifficulties[diff][Seed.IntRange(0, enemyDifficulties[diff].Count)], pos, (r, e) => {
            e.CStart(manager, r, root);
        });
    }

    public void IntervalStartWave() {
        Debug.Log("start");
        Save.rData.IncrementWaves();
        quantitativeRound.Enable();
        waitInterval.Disable();
    }

    public void EndWave() {
        Debug.Log("Wave ended ("+Waves+")");
        Save.rData.IncLinearTime(amount: 0.05f);
        waitInterval.Enable();
        if (Waves % 25 == 0) {
            World.WG.Expand(5);
        }
        if (Waves % 50 == 0) {
            World.WG.NewGoldPile();
        }
    }
}
