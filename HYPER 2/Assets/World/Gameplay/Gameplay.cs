using System.Collections.Generic;
using Helpers;
using UnityEngine;

public delegate void Doer();

public class Gameplay : MonoBehaviour
{
    private static Gameplay self;

    // Difficulty
    public int Waves {
        get { return Save.rData.Waves; }
    }
    public float LinearT {
        get { return Save.rData.LinearTime*2f; }
    }
    public int IntLinearT {
        get { return Mathf.FloorToInt(LinearT); }
    }

    public float AsymT {
        get { return Mathf.Sqrt(LinearT); }
    }
    public int IntAsymT {
        get { return Mathf.CeilToInt(Mathf.Sqrt(LinearT)); }
    }

    // Settings
    private int enemiesAmount = 1;
    public int EnemiesAmount {
        get { return Mathf.CeilToInt(enemiesAmount*Waves); }
    }

    public int RestoreHPPrice {
        get { 
            return Mathf.RoundToInt(LinearT*RestoreHPCost);
        }
    }

    public const int RestoreHPCost = 4;
    public const int MiniTowerLvlUpHPBonus = 10;

    // Unlocks
    public const int unlock2ndPath = 2;
    public const int unlock3rdPath = 3;
    public const int unlock4thPath = 4;
    public const int unlock1stWidePath = 5;
    public const int unlock2ndWidePath = 6;
    public const int unlock3rdWidePath = 7;
    public const int unlock4thWidePath = 8;

    //Frequencies
    private const int bossFrequency = 35;
    private const int goldPileFrequency = 50;
    private const int expandFrequency = 25;

    [SerializeField] private QuantitativeRound quantitativeRound;
    [SerializeField] private BossRound bossRound;
    [SerializeField] private Interval waitInterval;

    private Dictionary<int, List<Resources.Enemy>> enemyDifficulties = new Dictionary<int, List<Resources.Enemy>>();
    private Dictionary<int, List<Resources.Enemy>> bossEnemyDifficulties = new Dictionary<int, List<Resources.Enemy>>();

    // Dependencies
    private Manager manager;
    private World world;
    public World World {
        get { return world; }
    }

    public static int MoneyEarned(int lvl) {
        return Mathf.CeilToInt(Mathf.Sqrt(lvl)*2);
    }

    public void GenerationUnlockLogic(Doer doer) {
        var expansionsOwed = Mathf.FloorToInt(Save.rData.Waves / 25);
        world.WG.Expand(5*expansionsOwed);

        doer();

        var goldPilesOwed = Mathf.FloorToInt(Waves / 50); 
        for (int i = 1; i < goldPilesOwed; i++) {
            world.WG.NewGoldPile();
        }
    }

    private int maxDiff;
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
            var dict = enemyDifficulties;
            while (iter.MoveNext()) {
                if (iter.Current.Value.difficulty > maxDiff) {
                    maxDiff = iter.Current.Value.difficulty;
                }
                if (iter.Current.Value.type == EnemyType.BOSS) {
                    dict = bossEnemyDifficulties;
                }else {
                    dict = enemyDifficulties;
                }
                if (!dict.ContainsKey(iter.Current.Value.difficulty)) {
                    dict.Add(iter.Current.Value.difficulty, new List<Resources.Enemy>() {
                        iter.Current.Key
                    });
                }else {
                    dict[iter.Current.Value.difficulty].Add(iter.Current.Key);
                }
            }
        }

        waitInterval.Enable();
    }

    public Enemy SpawnEnemy(Vector3 pos, Direction main, Branch root) {
        var diff = Mathf.Clamp(Random.Range(IntLinearT-3, IntLinearT+1), 1, maxDiff);
        return world.InstantiateEnemy(enemyDifficulties[diff][Seed.IntRange(0, enemyDifficulties[diff].Count)], pos, (r, e) => {
            e.CStart(manager, r, root, main);
        });
    }

    private int BossDiff {
        get { 
            return Waves / bossFrequency;
        }
    }

    public Enemy SpawnBoss(Vector3 pos, Direction main, Branch root) {
        var diff = Mathf.Clamp(Random.Range(BossDiff-1, BossDiff+1), 1, maxDiff);
        return world.InstantiateEnemy(bossEnemyDifficulties[diff][Seed.IntRange(0, enemyDifficulties[diff].Count)], pos, (r, e) => {
            e.CStart(manager, r, root, main);
        });
    }

    public void IntervalStartWave() {
        Debug.Log("start");
        if (Waves % bossFrequency == 0 & Waves >= bossFrequency) {
            bossRound.Enable();
        }else {
            quantitativeRound.Enable();
        }
        waitInterval.Disable();
    }

    public void EndWave() {
        Debug.Log("Wave ended ("+Waves+")");
        Save.rData.IncLinearTime(amount: 0.05f);
        Save.rData.IncrementWaves();
        waitInterval.Enable();
        if (Waves % expandFrequency == 0 & Waves >= expandFrequency) {
            World.WG.Expand(5);
        }
   
        if (Waves % goldPileFrequency == 0 & Waves >= goldPileFrequency) {
            World.WG.NewGoldPile();
        }
        PublicEvent.SaveRun();
    }
}
