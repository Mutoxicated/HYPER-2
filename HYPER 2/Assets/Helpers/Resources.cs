using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Resources : MonoBehaviour
{
    public static Resources self;

    [Serializable]
    public enum Arsenal {
        FIRST_MT = 0,
        SECOND_MT
    }

    public enum Enemy {
        PROWLER = 0
    }

    public enum Bullet {
        NORMAL = 0
    }

    private static Dictionary<Arsenal, GameObject> arsenalPrefabs;
    private static Dictionary<Arsenal, StatsTemplate> arsenalStats;
    private static Dictionary<Arsenal, Sprite> arsenalSprites;
    private static Dictionary<Enemy, GameObject> enemyPrefabs;
    private static Dictionary<Bullet, GameObject> bulletPrefabs;

    [SerializeField] private Sprite empty;
    [SerializeField] private Sprite locked;

    public static Sprite Empty {
        get { return self.empty; }
    }

    public static Sprite Locked {
        get { return self.locked; }
    }

    public void DStart() {
        if (arsenalPrefabs == null) {
            arsenalPrefabs = new Dictionary<Arsenal, GameObject>();
            var gos = UnityEngine.Resources.LoadAll<GameObject>("Arsenal");
            Assert.IsNotNull(gos, "Arsenal folder in Helpers/Resources either doesn't exist");
            for (int i = 0; i < gos.Length; i++) {
                arsenalPrefabs.Add((Arsenal)i, gos[i]);
            } 
        }
        if (enemyPrefabs == null) {
            enemyPrefabs = new Dictionary<Enemy, GameObject>();
            var gos = UnityEngine.Resources.LoadAll<GameObject>("Enemy");
            Assert.IsNotNull(gos, "Enemy folder in Helpers/Resources doesn't exist");
            Debug.Log("Uh "+gos.Length);
            for (int i = 0; i < gos.Length; i++) {
                enemyPrefabs.Add((Enemy)i, gos[i]);
            } 
        }
        if (bulletPrefabs == null) {
            bulletPrefabs = new Dictionary<Bullet, GameObject>();
            var gos = UnityEngine.Resources.LoadAll<GameObject>("Bullet");
            Assert.IsNotNull(gos, "Bullet folder in Helpers/Resources doesn't exist");
            for (int i = 0; i < gos.Length; i++) {
                bulletPrefabs.Add((Bullet)i, gos[i]);
            }
        }
        if (arsenalSprites == null) {
            arsenalSprites = new Dictionary<Arsenal, Sprite>();
            var gos = UnityEngine.Resources.LoadAll<Sprite>("Arsenal");
            Assert.IsNotNull(gos, "Arsenal folder in Helpers/Resources doesn't exist");
            for (int i = 0; i < gos.Length; i++) {
                arsenalSprites.Add((Arsenal)i, gos[i]);
            }
        }
        if (arsenalStats == null) {
            arsenalStats = new Dictionary<Arsenal, StatsTemplate>();
            var gos = UnityEngine.Resources.LoadAll<StatsTemplate>("Arsenal");
            Assert.IsNotNull(gos, "Arsenal folder in Helpers/Resources doesn't exist");
            for (int i = 0; i < gos.Length; i++) {
                arsenalStats.Add((Arsenal)i, gos[i]);
            }
        }
        if (self == null) {
            self = this;
        }
    }

    public static GameObject GetBullet(Bullet bullet) {
        return bulletPrefabs[bullet];
    }

    public static GameObject GetEnemy(Enemy enemy) {
        return enemyPrefabs[enemy];
    }

    public static GameObject GetArsenal(Arsenal arsenal) {
        return arsenalPrefabs[arsenal];
    }

    public static Sprite GetArsenalSprite(Arsenal arsenal) {
        return arsenalSprites[arsenal];
    }

    public static StatsTemplate GetStatTemplate(Arsenal arsenal) {
        return arsenalStats[arsenal];
    }
}
