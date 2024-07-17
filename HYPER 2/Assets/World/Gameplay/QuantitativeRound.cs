using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuantitativeRound : MonoBehaviour
{
    [SerializeField] private Gameplay gameplay;

    private float spawnRate = 0.5f;
    private int spawns = 0;
    private int totalSpawns = 0;
    
    private Vector2 weirdSpawnRateRange = new Vector2(0.2f, 0.4f);
    private Vector2 weirdSpawnFrequencyRange = new Vector2(3, 6);
    private float weirdSpawnRate;
    private int weirdSpawnFrequency;
    private bool usedWeirdSpawn = false;


    private float time = 0f;

    public void Enable() {
        weirdSpawnRate = Seed.NextFloat(weirdSpawnRateRange.x, weirdSpawnRateRange.y);
        weirdSpawnFrequency = Seed.IntRange((int)weirdSpawnFrequencyRange.x, (int)weirdSpawnFrequencyRange.y);
    }

    public void Update() {
        time += Time.deltaTime;
        if (usedWeirdSpawn && time >= weirdSpawnRate) {
            usedWeirdSpawn = false;
            //spawn
        }
        if (time >= spawnRate) {
            spawns++;
            totalSpawns++;
            //spawn
            if (spawns >= weirdSpawnFrequency) {
                spawns = 0;
                usedWeirdSpawn = true;
            }
        }

        if (totalSpawns > gameplay.enemiesAmount) {
            enabled = false;
            gameplay.EndWave();
        }
    }
}
