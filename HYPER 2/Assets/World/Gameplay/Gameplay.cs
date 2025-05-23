using System.Collections.Generic;
using System.Linq;
using Helpers;
using UnityEngine;
using static EnemyType;

public delegate void Doer();

public class Gameplay : MonoBehaviour
{
    private static Gameplay self;

    // Difficulty
    public static int Waves {
        get { return Save.rData.Waves; }
    }
    public static float LinearT {
        get { return Save.rData.LinearTime*2f; }
    }
    public static int IntLinearT {
        get { return Mathf.FloorToInt(LinearT); }
    }

    public static float AsymT {
        get { return Mathf.Sqrt(LinearT); }
    }
    public static int IntAsymT {
        get { return Mathf.CeilToInt(Mathf.Sqrt(LinearT)); }
    }
    public static int EnemyLvl {
        get { return Mathf.FloorToInt(LinearT*1.5f); }
    }

    // Settings
    private int enemiesAmount = 1;
    public int EnemiesAmount {
        get { return Mathf.CeilToInt(enemiesAmount*Waves)+4*(Save.rData.Paths.Count-1); }
    }

    public int RestoreHPPrice {
        get { 
            return Mathf.RoundToInt(LinearT*RestoreHPCost);
        }
    }

    public const int ResearchXPStep = 15;
    public const int ResearchSlotCost = 10;
    public const int ResearchSlotLimit = 5;
    public const int TowerLvlUpHPBonus = 20;
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
    private const int bossFrequency = 30;
    private const int goldPileFrequency = 50;
    private const int expandFrequency = 25;

    [SerializeField] private EnemyChanceEvaluator ece;
    [SerializeField] private QuantitativeRound quantitativeRound;
    [SerializeField] private BossRound bossRound;
    [SerializeField] private InvasionRound invasionRound;
    [SerializeField] private Interval waitInterval;

    public static Dictionary<EnemyType, Dictionary<int, List<Resources.Enemy>>> enemyData = new Dictionary<EnemyType, Dictionary<int, List<Resources.Enemy>>>();

    private Vector2Int minMaxEnemyRange;

    // Dependencies
    private Manager manager;
    private World world;
    public World World {
        get { return world; }
    }

    public static int MoneyEarned(int lvl) {
        return Mathf.CeilToInt(Mathf.Sqrt(lvl)*2f);
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

    public static int maxDiff;
    public void DStart(Manager manager) {
        self = this;
        this.manager = manager;
        world = manager.World;

        enemyData.Clear();

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

        SetMinMaxEnemyRange(0, 0);
        ece.GiveEnemyRange(minMaxEnemyRange);
        ece.UpdateTime();

        waitInterval.Enable();
    }

    public void SetMinMaxEnemyRange(int dMin, int dMax) {
        minMaxEnemyRange.x = IntLinearT-4+dMin;
        minMaxEnemyRange.y = IntLinearT+dMax;
    }

    public List<(Resources.Enemy enemy, int chance)> GetEnemyContestants() {
        List<(Resources.Enemy, int)> enemies = new List<(Resources.Enemy, int)>();

        var clampedMinDiff = Mathf.Clamp(minMaxEnemyRange.x, 1, maxDiff);
        var clampedMaxDiff = Mathf.Clamp(minMaxEnemyRange.y, 1, maxDiff);

        int middle = Mathf.RoundToInt((float)clampedMinDiff+(float)clampedMaxDiff/2f);

        var maxAngle = Mathf.PI;
        var angleStep = maxAngle/middle;

        // grab from 1/3 to 1/1.5 of the enemies from each difficulty bracket
        for (int i = clampedMinDiff; i <= clampedMaxDiff; i++) {
            float a = General.Remap(-1, 1, 0, 1, Mathf.Sin(270*Mathf.Deg2Rad+i*angleStep));
            
            float enemiesLength = (float)enemyData[COMMON][i].Count;
            int enemiesToGet = Mathf.CeilToInt(Mathf.Lerp(Mathf.CeilToInt(enemiesLength/3f), Mathf.CeilToInt(enemiesLength/1.5f), a));

            enemies.AddRange(General.RandomCollectTuple(enemyData[COMMON][i], enemiesToGet, v => ece.GetEnemyChance(v)));
        }

        return enemies;
    }

    public Resources.Enemy ChooseEnemy() {
        var enemies = GetEnemyContestants();
        var chanceLocations = new List<(int min, int max)>();

        int max = 0;
        foreach (var value in enemies) {
            int previousMax = max;
            max += value.chance;
            chanceLocations.Add((previousMax, max));
        }
        
        int random = Random.Range(0, max);

        int index = 0;
        for (int i = 0; i < enemies.Count; i++) {
            if (random > chanceLocations[i].min && random <= chanceLocations[i].max) {
                index = i;
                break;
            }
        }
        return enemies[Mathf.Clamp(index, 0, enemies.Count-1)].enemy;
    }

    public Enemy SpawnEnemy(Vector3 pos, Direction main, Branch root) {  
        return world.InstantiateEnemy(ChooseEnemy(), pos, (r, e) => {
            e.CStart(manager, r, root, main);
        });
    }

    public Enemy SpawnUncommon(Vector3 pos, Direction main, Branch root) {
        return world.InstantiateEnemy(ChooseEnemy(), pos, (r, e) => {
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
        ece.UpdateTime();
        SetMinMaxEnemyRange(0, 0);
        ece.GiveEnemyRange(minMaxEnemyRange);
    }
}
