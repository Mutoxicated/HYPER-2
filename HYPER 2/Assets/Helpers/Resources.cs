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
        HEALINATOR,
        KEEPER
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

    private Dictionary<Misc, GameObject> miscs;

    private Dictionary<Arsenal, GameObject> arsenalPrefabs;
    private Dictionary<Arsenal, StatsTemplate> arsenalStats;
    private Dictionary<Arsenal, MinitowerInfo> mtInfo;
    private Dictionary<MTUpgradeType, MTUpgrade> mtUpgrades;
    private Dictionary<MTUpgradeType, Sprite> mtUpgradeSprites;
    private Dictionary<Enemy, StatsTemplate> enemyStatTemplates;
    private Dictionary<Bullet, StatsTemplate> bulletStats;
    private Dictionary<Arsenal, Sprite> arsenalSprites;
    private Dictionary<Enemy, GameObject> enemyPrefabs;
    private Dictionary<Enemy, EnemyStats> enemyStats;
    private Dictionary<Bullet, GameObject> bulletPrefabs;

    [SerializeField] private Sprite empty;
    [SerializeField] private Sprite locked;

    public static Sprite Empty {
        get { return self.empty; }
    }

    public static Sprite Locked {
        get { return self.locked; }
    }

    public void DStart() {
        self = this;
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
        if (mtUpgradeSprites == null) {
            mtUpgradeSprites = new Dictionary<MTUpgradeType, Sprite>();
            var gos = UnityEngine.Resources.LoadAll<Sprite>("MTUpgrades");
            Assert.IsNotNull(gos, "MTUpgrades folder in Helpers/Resources doesn't exist");
            for (int i = 0; i < gos.Length; i++) {
                mtUpgradeSprites.Add((MTUpgradeType)i, gos[i]);
            }
        }
    }

    public static GameObject GetMisc(Misc misc) {
        return self.miscs[misc];
    }

    public static GameObject GetBullet(Bullet bullet) {
        return self.bulletPrefabs[bullet];
    }

    public static StatsTemplate GetBulletStats(Bullet bullet) {
        return self.bulletStats[bullet];
    }

    public static GameObject GetEnemy(Enemy enemy) {
        return self.enemyPrefabs[enemy];
    }

    public static GameObject GetArsenal(Arsenal arsenal) {
        return self.arsenalPrefabs[arsenal];
    }

    public static Sprite GetArsenalSprite(Arsenal arsenal) {
        return self.arsenalSprites[arsenal];
    }

    public static StatsTemplate GetStatTemplate(Arsenal arsenal) {
        return self.arsenalStats[arsenal];
    }

    public static MinitowerInfo GetMTInfo(Arsenal arsenal) {
        return self.mtInfo[arsenal];
    }

    public static MTUpgrade GetMTUpgrade(MTUpgradeType arsenal) {
        return self.mtUpgrades[arsenal];
    }

    public static Sprite GetMTUpgradeSprite(MTUpgradeType arsenal) {
        return self.mtUpgradeSprites[arsenal];
    }

    public static EnemyStats GetEnemyStats(Enemy enemy) {
        return self.enemyStats[enemy];
    }

        public static StatsTemplate GetEnemyStatTemplates(Enemy arsenal) {
        return self.enemyStatTemplates[arsenal];
    }
    
    public static IEnumerator<KeyValuePair<Enemy, EnemyStats>> IterEnemyStats() {
        return self.enemyStats.GetEnumerator();
    }
}
