using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChanceEvaluator : MonoBehaviour
{
    private Vector2Int minMaxEnemyRange = new Vector2Int(0, 0);
    public Dictionary<Resources.Enemy, float> sinWaveModifiers = new Dictionary<Resources.Enemy, float>();

    public void GiveEnemyRange(Vector2Int value) {
        if (value.y > minMaxEnemyRange.y) {
            var diff = Mathf.Clamp(value.y, 1, Gameplay.maxDiff);

            foreach (var enemy in Gameplay.enemyData[EnemyType.COMMON][diff]) {
                if (sinWaveModifiers.ContainsKey(enemy)) { continue; }
                sinWaveModifiers.Add(enemy, 0);
                Save.rData.AddEnemyChance(enemy, Random.Range(0.1f, 0.3f));
            }
            if (Gameplay.enemyData[EnemyType.UNCOMMON].ContainsKey(diff)) {
                foreach (var enemy in Gameplay.enemyData[EnemyType.UNCOMMON][diff]) {
                    if (sinWaveModifiers.ContainsKey(enemy)) { continue; }
                    sinWaveModifiers.Add(enemy, 0);
                    Save.rData.AddEnemyChance(enemy, Random.Range(0.1f, 0.3f));
                }
            }
           
        }
        minMaxEnemyRange = value;
    }

    public int GetEnemyChance(Resources.Enemy enemy) {
        return Mathf.RoundToInt(Mathf.Lerp(Resources.GetEnemyStats(enemy).startChance, 100, sinWaveModifiers[enemy]));
    }

    float cachedTime = 0;
    public void UpdateTime() {
        for (int i = 0; i < Save.rData.EnemyKeys.Count; i++) {
            Save.rData.IncEnemyChanceTime(i);
            cachedTime = Save.rData.GetEnemyChanceTime(i).Get();
            sinWaveModifiers[Save.rData.EnemyKeys[i]] = Mathf.Sin(Mathf.Deg2Rad*(cachedTime/2f));
            if (cachedTime > 360) {
                Save.rData.LoopEnemyChanceTime(i);
            }
        }
    }
}
