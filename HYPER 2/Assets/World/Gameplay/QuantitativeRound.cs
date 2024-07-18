using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRound {
    Enemy Spawn(Branch root, Vector3 pos);
}

public class QuantitativeRound : MonoBehaviour, IRound
{
    [SerializeField] private Gameplay gameplay;

    private float spawnRate = 0.5f;
    private int spawns = 0;
    private int totalSpawns = 0;
    private List<Interval> spawnIntervals = new List<Interval>();
    
    //private Vector2 weirdSpawnRateRange = new Vector2(0.2f, 0.4f);
    //private Vector2 weirdSpawnFrequencyRange = new Vector2(3, 6);
    //private float weirdSpawnRate;
    //private int weirdSpawnFrequency;
    //private bool usedWeirdSpawn = false;

    private float time = 0f;

    private void Awake() {
    }

    private void UpdateOnSpawn() {
        totalSpawns++;
    }

    public Enemy Spawn(Branch root, Vector3 pos) {
        var enemy = gameplay.SpawnEnemy(pos, root);
        UpdateOnSpawn();
        return enemy;
    }

    public void Enable() {
        //weirdSpawnRate = Seed.NextFloat(weirdSpawnRateRange.x, weirdSpawnRateRange.y);
        //weirdSpawnFrequency = Seed.IntRange((int)weirdSpawnFrequencyRange.x, (int)weirdSpawnFrequencyRange.y);
        spawnIntervals.Clear();
        foreach (var path in gameplay.World.WG.Paths) {
            spawnIntervals.Add(path.StartSpawning(this));
        }
    }

    public void Update() {
        time += Time.deltaTime;

        if (totalSpawns > gameplay.enemiesAmount) {
            enabled = false;
            foreach (var path in gameplay.World.WG.Paths) {
                path.StopSpawning();
            }
            totalSpawns = 0;
            time = 0f;
            gameplay.EndWave();

        }
    }
}
