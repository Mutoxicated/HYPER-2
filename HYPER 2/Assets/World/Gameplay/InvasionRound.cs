using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InvasionRound : MonoBehaviour, IRound
{
    [SerializeField] private Gameplay gameplay;
    [SerializeField] private Interval interval;

    private Vector2 spawnRate = new Vector2(0.5f, 1.5f);
    private float SpawnRate {
        get { return Seed.NextFloat(spawnRate.x, spawnRate.y); }
    }
    private int totalSpawns = 0;
    private List<Interval> spawnIntervals = new List<Interval>();
    
    private void UpdateOnSpawn() {
        totalSpawns++;
    }

    public void SpawnUncommon() {
        gameplay.World.WG.RandomPath.CustomSpawn((r, m, p) => {
            var enemy = gameplay.SpawnUncommon(p, m, r);
            UpdateOnSpawn();
            return enemy;
        });
    }

    public Enemy Spawn(Branch root, Direction main, Vector3 pos) {
        var enemy = gameplay.SpawnEnemy(pos, main, root);
        UpdateOnSpawn();
        return enemy;
    }

    public void Enable() {
        interval.Enable();
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
        if (totalSpawns >= gameplay.EnemiesAmount) {
            if (spawnIntervals[0].enabled) {
                foreach (var path in gameplay.World.WG.Paths) {
                    path.StopSpawning();
                }
            }
            if (gameplay.World.Enemies.Count == 0) {
                enabled = false;
                totalSpawns = 0;
                interval.Disable();
                gameplay.EndWave();
            }
        }
    }
}
