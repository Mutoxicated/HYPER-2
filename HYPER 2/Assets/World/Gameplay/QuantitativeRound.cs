using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IRound {
    Enemy Spawn(Branch root, Direction main, Vector3 pos);
}

public delegate Enemy Spawn(Branch root, Direction main, Vector3 nbs);

public class QuantitativeRound : MonoBehaviour, IRound
{
    [SerializeField] private Gameplay gameplay;

    private Vector2 spawnRate = new Vector2(0.5f, 1.5f);
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

    private void UpdateOnSpawn() {
        totalSpawns++;
        //Debug.Log("Update On Spawn ("+totalSpawns+")");
    }

    public Enemy Spawn(Branch root, Direction main, Vector3 pos) {
        var enemy = gameplay.SpawnEnemy(pos, main, root);
        UpdateOnSpawn();
        return enemy;
    }

    public void Enable() {
        //weirdSpawnRate = Seed.NextFloat(weirdSpawnRateRange.x, weirdSpawnRateRange.y);
        //weirdSpawnFrequency = Seed.IntRange((int)weirdSpawnFrequencyRange.x, (int)weirdSpawnFrequencyRange.y);
        spawnIntervals.Clear();
        foreach (var path in gameplay.World.WG.Paths) {
            spawnIntervals.AddRange(path.StartSpawning(this));
        }
        foreach (var spawnInterval in spawnIntervals) {
            spawnInterval.ChangeInterval(SpawnRate);
        }
        enabled = true;
    }

    public void Update() {
        //Debug.Log("Enemies Amount: "+gameplay.enemiesAmount);

        if (totalSpawns >= gameplay.EnemiesAmount) {
            if (spawnIntervals[0].enabled) {
                foreach (var path in gameplay.World.WG.Paths) {
                    path.StopSpawning();
                }
            }
            if (gameplay.World.Enemies.Count == 0) {
                enabled = false;
                totalSpawns = 0;
                gameplay.EndWave();
            }
        }
    }
}
