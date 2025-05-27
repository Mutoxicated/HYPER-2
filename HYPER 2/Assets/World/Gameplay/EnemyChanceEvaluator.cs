using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;

public class EnemyChanceEvaluator : MonoBehaviour
{
    private Vector2Int minMaxEnemyRange = new Vector2Int(0, 0);
    public Dictionary<Resources.Enemy, float> sinWaveModifiers = new Dictionary<Resources.Enemy, float>();
    
    [SerializeField] private List<float> enemyChanceTimes = new List<float>();
    [SerializeField] private List<float> enemyChanceSteps = new List<float>();

    public int enemiesEvaluated
    {
        get { return enemyChanceTimes.Count; }
    }

    public void CStart()
    {
        enemyChanceTimes = Save.rData.EnemyChanceTimes;
        enemyChanceSteps = Save.rData.EnemyChanceSteps;

        PublicEvent.onSaveRun += data =>
        {
            Save.rData.EnemyChanceTimes = enemyChanceTimes;
            Save.rData.EnemyChanceSteps = enemyChanceSteps;
        };
    }

    public void GiveEnemyRange(Vector2Int value)
    {
        if (value.y > minMaxEnemyRange.y)
        {
            var diff = Mathf.Clamp(value.y, 1, Gameplay.maxDiff);

            foreach (var enemy in Gameplay.enemyData[EnemyType.COMMON][diff])
            {
                if (sinWaveModifiers.ContainsKey(enemy)) { continue; }
                sinWaveModifiers.Add(enemy, 0);
                enemyChanceTimes.Add(0);
                enemyChanceSteps.Add(Random.Range(0.1f, 0.3f));
            }
            if (Gameplay.enemyData[EnemyType.UNCOMMON].ContainsKey(diff))
            {
                foreach (var enemy in Gameplay.enemyData[EnemyType.UNCOMMON][diff])
                {
                    if (sinWaveModifiers.ContainsKey(enemy)) { continue; }
                    sinWaveModifiers.Add(enemy, 0);
                    enemyChanceTimes.Add(0);
                    enemyChanceSteps.Add(Random.Range(0.1f, 0.3f));
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
        for (int i = 0; i < enemyChanceSteps.Count; i++) {
            enemyChanceTimes[i] += enemyChanceSteps[i];
            cachedTime = enemyChanceTimes[i];
            sinWaveModifiers[(Resources.Enemy)i] = Mathf.Sin(Mathf.Deg2Rad*(cachedTime/2f));
            if (cachedTime > 360) {
                enemyChanceTimes[i] -= 360;
            }
        }
    }
}
