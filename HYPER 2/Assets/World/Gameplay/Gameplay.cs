using System.Collections.Generic;
using Helpers;
using UnityEngine;
using static EnemyType;

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
    public int EnemyLvl {
        get { return Mathf.FloorToInt(1+0.5f*LinearT*0.25f); }
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

    public const int RestoreHPCost = 2;
    public const int MiniTowerLvlUpHPBonus = 10;
    public const int BossPaycheck = 20;

    // Unlocks
    public const int unlock2ndPath = 6;
    public const int unlock3rdPath = 14;
    public const int unlock4thPath = 20;
    public const int unlock1stWidePath = 25;
    public const int unlock2ndWidePath = 34;
    public const int unlock3rdWidePath = 43;
    public const int unlock4thWidePath = 51;

    //Frequencies
    private const int uncommonFrequency = 15;
    private const int bossFrequency = 35;
    private const int goldPileFrequency = 50;
    private const int expandFrequency = 25;

    [SerializeField] private QuantitativeRound quantitativeRound;
    [SerializeField] private BossRound bossRound;
    [SerializeField] private InvasionRound invasionRound;
    [SerializeField] private Interval waitInterval;

    private Dictionary<EnemyType, Dictionary<int, List<Resources.Enemy>>> enemyData = new Dictionary<EnemyType, Dictionary<int, List<Resources.Enemy>>>();

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

    private void FillDict(EnemyStats es, Resources.Enemy enemy) {
        if (!enemyData[es.type].ContainsKey(es.difficulty)) {
            enemyData[es.type].Add(es.difficulty, new List<Resources.Enemy>() {
                enemy
            });
        }else {
            enemyData[es.type][es.difficulty].Add(enemy);
        }
    }

    private int maxDiff;
    public void DStart(Manager manager) {
        self = this;
        this.manager = manager;
        world = manager.World;

        var iter = Resources.IterEnemyStats();
        using (iter) {
            EnemyStats es;
            while (iter.MoveNext()) {
                es = iter.Current.Value;
                if (es.difficulty > maxDiff) {
                    maxDiff = es.difficulty;
                }
                if (!enemyData.ContainsKey(es.type)) {
                    enemyData.Add(es.type, new Dictionary<int, List<Resources.Enemy>>());
                }
                FillDict(es, iter.Current.Key);
            }
        }

        waitInterval.Enable();
    }

    public Enemy SpawnEnemy(Vector3 pos, Direction main, Branch root) {
        var diff = Mathf.Clamp(Random.Range(IntLinearT-3, IntLinearT+1), 1, maxDiff);
        return world.InstantiateEnemy(enemyData[COMMON][diff][Seed.IntRange(0, enemyData[COMMON][diff].Count)], pos, (r, e) => {
            e.CStart(manager, r, root, main);
        });
    }

    public Enemy SpawnUncommon(Vector3 pos, Direction main, Branch root) {
        var diff = Mathf.Clamp(Random.Range(IntLinearT-3, IntLinearT+1), 1, maxDiff);
        return world.InstantiateEnemy(enemyData[COMMON][diff][Seed.IntRange(0, enemyData[COMMON][diff].Count)], pos, (r, e) => {
            e.CStart(manager, r, root, main);
        });
    }

    private int BossDiff {
        get { 
            return Waves / bossFrequency;
        }
    }

    private void NextPath() {
        manager.GUI.PushNotif("<color=#ff0000>A new path has emerged...");
        world.WG.CreateNextPath();
    }

    public Enemy SpawnBoss(Vector3 pos, Direction main, Branch root) {
        var diff = Mathf.Clamp(Random.Range(BossDiff-1, BossDiff+1), 1, maxDiff);
        return world.InstantiateEnemy(enemyData[BOSS][diff][Seed.IntRange(0, enemyData[BOSS][diff].Count)], pos, (r, e) => {
            e.CStart(manager, r, root, main);
        });
    }

    public bool FrequencyCheck(int num, int freq) {
        return num % freq == 0 & num >= freq;
    }

    public void IntervalStartWave() {
        Debug.Log("start");
        if (FrequencyCheck(Waves, bossFrequency)) {
            Player.SetMoneyRelative(BossPaycheck*BossDiff);
            bossRound.Enable();
        }else if (Waves > bossFrequency && Waves % uncommonFrequency == 0) {
            invasionRound.Enable();
        }else{
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

        switch (Waves) {
            case unlock2ndPath:
                NextPath();
                break;
            case unlock3rdPath:
                NextPath();
                break;
            case unlock4thPath:
                NextPath();
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
        PublicEvent.SaveRun();
    }
}
