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
        RADMOL,
        SECOND_MT,
        WATCHER,
        BOOMER,
        SKYSCRAPER,
        HEALINATOR
    }

    public enum Enemy {
        MOG = 0,
        PASATEMPO,
        PROWLER,
        SOAS,
        UFO,
        GUARDIAN
    }

    public enum Bullet {
        NORMAL = 0,
        RADIATING,
        WIND,
        BOOM
    }

    [Serializable]
    public enum MTUpgradeType {
        MORE_HEALTH,
        MORE_SPEED,
        MORE_DAMAGE,
        MORE_RANGE,
        MORE_RATE,
        MORE_ARMOR,
        MORE_IMMUNITY,
        MORE_CHANCE,
        MORE_AMOUNT,
        INFECT_WATER,
        INFECT_FIRE,
        INFECT_RADIATION
    }

    public enum Misc {
        Line
    }

    private static Dictionary<Misc, GameObject> miscs;

    private static Dictionary<Arsenal, GameObject> arsenalPrefabs;
    private static Dictionary<Arsenal, StatsTemplate> arsenalStats;
    private static Dictionary<Arsenal, MinitowerInfo> mtInfo;
    private static Dictionary<MTUpgradeType, MTUpgrade> mtUpgrades;
    private static Dictionary<Enemy, StatsTemplate> enemyStatTemplates;
    private static Dictionary<Bullet, StatsTemplate> bulletStats;
    private static Dictionary<Arsenal, Sprite> arsenalSprites;
    private static Dictionary<Enemy, GameObject> enemyPrefabs;
    private static Dictionary<Enemy, EnemyStats> enemyStats;
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
        if (miscs == null) {
            miscs = new Dictionary<Misc, GameObject>();
            var gos = UnityEngine.Resources.LoadAll<GameObject>("Misc");
            Assert.IsNotNull(gos, "Misc folder in Helpers/Resources doesn't exist");
            for (int i = 0; i < gos.Length; i++) {
                miscs.Add((Misc)i, gos[i]);
            } 
        }

        if (arsenalPrefabs == null) {
            arsenalPrefabs = new Dictionary<Arsenal, GameObject>();
            var gos = UnityEngine.Resources.LoadAll<GameObject>("Arsenal");
            Assert.IsNotNull(gos, "Arsenal folder in Helpers/Resources doesn't exist");
            for (int i = 0; i < gos.Length; i++) {
                arsenalPrefabs.Add((Arsenal)i, gos[i]);
            } 
        }
        if (enemyPrefabs == null) {
            enemyPrefabs = new Dictionary<Enemy, GameObject>();
            var gos = UnityEngine.Resources.LoadAll<GameObject>("Enemy");
            Assert.IsNotNull(gos, "Enemy folder in Helpers/Resources doesn't exist");
            //Debug.Log("Uh "+gos.Length);
            for (int i = 0; i < gos.Length; i++) {
                enemyPrefabs.Add((Enemy)i, gos[i]);
            } 
        }
        if (enemyStats == null) {
            enemyStats = new Dictionary<Enemy, EnemyStats>();
            var gos = UnityEngine.Resources.LoadAll<EnemyStats>("Enemy");
            Assert.IsNotNull(gos, "Enemy folder in Helpers/Resources doesn't exist");
            //Debug.Log("Uh "+gos.Length);
            for (int i = 0; i < gos.Length; i++) {
                enemyStats.Add((Enemy)i,  gos[i]);
            } 
        }
        if (enemyStatTemplates == null) {
            enemyStatTemplates = new Dictionary<Enemy, StatsTemplate>();
            var gos = UnityEngine.Resources.LoadAll<StatsTemplate>("Enemy");
            Assert.IsNotNull(gos, "Enemy folder in Helpers/Resources doesn't exist");
            //Debug.Log("Uh "+gos.Length);
            for (int i = 0; i < gos.Length; i++) {
                enemyStatTemplates.Add((Enemy)i, gos[i]);
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
        if (bulletStats == null) {
            bulletStats = new Dictionary<Bullet, StatsTemplate>();
            var gos = UnityEngine.Resources.LoadAll<StatsTemplate>("Bullet");
            Assert.IsNotNull(gos, "Bullet folder in Helpers/Resources doesn't exist");
            for (int i = 0; i < gos.Length; i++) {
                bulletStats.Add((Bullet)i, gos[i]);
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
        if (mtInfo == null) {
            mtInfo = new Dictionary<Arsenal, MinitowerInfo>();
            var gos = UnityEngine.Resources.LoadAll<MinitowerInfo>("Arsenal");
            Assert.IsNotNull(gos, "Arsenal folder in Helpers/Resources doesn't exist");
            for (int i = 0; i < gos.Length; i++) {
                mtInfo.Add((Arsenal)i, gos[i]);
            }
        }
         if (mtUpgrades == null) {
            mtUpgrades = new Dictionary<MTUpgradeType, MTUpgrade>();
            var gos = UnityEngine.Resources.LoadAll<MTUpgrade>("MTUpgrades");
            Assert.IsNotNull(gos, "MTUpgrades folder in Helpers/Resources doesn't exist");
            for (int i = 0; i < gos.Length; i++) {
                mtUpgrades.Add((MTUpgradeType)i, gos[i]);
            }
        }
        if (self == null) {
            self = this;
        }
    }

    public static GameObject GetMisc(Misc misc) {
        return miscs[misc];
    }

    public static GameObject GetBullet(Bullet bullet) {
        return bulletPrefabs[bullet];
    }

    public static StatsTemplate GetBulletStats(Bullet bullet) {
        return bulletStats[bullet];
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

    public static MinitowerInfo GetMTInfo(Arsenal arsenal) {
        return mtInfo[arsenal];
    }

    public static MTUpgrade GetMTInfo(MTUpgradeType arsenal) {
        return mtUpgrades[arsenal];
    }

    public static EnemyStats GetEnemyStats(Enemy enemy) {
        return enemyStats[enemy];
    }

        public static StatsTemplate GetEnemyStatTemplates(Enemy arsenal) {
        return enemyStatTemplates[arsenal];
    }
    
    public static IEnumerator<KeyValuePair<Enemy, EnemyStats>> IterEnemyStats() {
        return enemyStats.GetEnumerator();
    }
}
