using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Powerup {
    public List<Resources.MTUpgradeType> upgrades = new List<Resources.MTUpgradeType>();
    public List<int> rarities = new List<int>();

    public Powerup(Resources.Arsenal arsenalType, MinitowerInfo info, List<Resources.MTUpgradeType> takenUpgrades) {
        var availableUpgrades = info.availableUpgrades.ToList();

        for (int i = 0; i < takenUpgrades.Count; i++) {
            for (int j = 0; j < availableUpgrades.Count; j++) {
                if (takenUpgrades[i] == availableUpgrades[j] && !Resources.GetMTUpgrade(takenUpgrades[i]).stackable) {
                    availableUpgrades.RemoveAt(j);
                    break;
                }
            }
        }

        for (int i = availableUpgrades.Count-1; i >= 0; i--) {
            var random = Seed.IntRange(0, availableUpgrades.Count-1);
            upgrades.Add(availableUpgrades[random]);
            rarities.Add(Mathf.RoundToInt((float)random/(float)info.availableUpgrades.Count*100f));
            availableUpgrades.RemoveAt(random);
            if (upgrades.Count >= 3) {
                return;
            }
        }
    }
}