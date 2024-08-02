using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossRound : MonoBehaviour, IRound
{
    [SerializeField] private Gameplay gameplay;

    private Enemy boss;

    private Vector2 spawnRate = new Vector2(0.5f, 2f);
    private float SpawnRate {
        get { return Seed.NextFloat(spawnRate.x, spawnRate.y); }
    }
    private int totalSpawns = 0;
    private List<Interval> spawnIntervals = new List<Interval>();
    
    //private Vector2 weirdSpawnRateRange = new Vector2(0.2f, 0.4f);
    //private Vector2 weirdSpawnFrequencyRange = new Vector2(3, 6);
    //private float weirdSpawnRate;
    //private int weirdSpawnFrequency;
    //private bool usedWeirdSpawn = false;

    private float time = 0f;

    private void UpdateOnSpawn() {
        totalSpawns++;
        //Debug.Log("Update On Spawn ("+totalSpawns+")");
    }

    public Enemy Spawn(Branch root, Direction main, Vector3 pos, bool boss = false) {
        var enemy = boss ? gameplay.SpawnBoss(pos, main, root) : gameplay.SpawnEnemy(pos, main, root);
        if (boss)
            this.boss = enemy;
        else
            UpdateOnSpawn();
        return enemy;
    }

    public Enemy SpawnBoss(Branch root, Direction main, Vector3 pos) {
        return gameplay.SpawnBoss(pos, main, root);
    } 

    public void Enable() {
        //weirdSpawnFrequency = Seed.IntRange((int)weirdSpawnFrequencyRange.x, (int)weirdSpawnFrequencyRange.y);
        spawnIntervals.Clear();
        foreach (var path in gameplay.World.WG.Paths) {
            spawnIntervals.Concat(path.StartSpawning(this));
        }
        gameplay.World.WG.Paths[Seed.IntRange(0, gameplay.World.WG.Paths.Count-1)].SpawnBoss();
        foreach (var spawnInterval in spawnIntervals) {
            spawnInterval.ChangeInterval(SpawnRate);
        }
        enabled = true;
    }

    public void Update() {
        time += Time.deltaTime;
        //Debug.Log("Enemies Amount: "+gameplay.enemiesAmount);

        if (totalSpawns >= gameplay.EnemiesAmount & boss == null) {
            if (spawnIntervals[0].enabled) {
                foreach (var path in gameplay.World.WG.Paths) {
                    path.StopSpawning();
                }
            }
            if (gameplay.World.Enemies.Count == 0) {
                enabled = false;
                totalSpawns = 0;
                time = 0f;
                gameplay.EndWave();
            }
        }
    }
}
