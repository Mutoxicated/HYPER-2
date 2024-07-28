using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private void UpdateOnSpawn() {
        totalSpawns++;
        //Debug.Log("Update On Spawn ("+totalSpawns+")");
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
            spawnIntervals.Concat(path.StartSpawning(this));
        }
        enabled = true;
    }

    public void Update() {
        time += Time.deltaTime;
        //Debug.Log("Enemies Amount: "+gameplay.enemiesAmount);

        if (totalSpawns > gameplay.EnemiesAmount) {
            Debug.Log("STOP");
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
