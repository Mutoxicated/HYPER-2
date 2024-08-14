using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Powerup {
    public List<Resources.MTUpgradeType> upgrades = new List<Resources.MTUpgradeType>();

    public Powerup(Resources.Arsenal arsenalType, MinitowerInfo info) {
        var availableUpgrades = info.availableUpgrades;

        var takenUpgrades = Save.rData.GetTakenUpgrades(arsenalType);
        for (int i = 0; i < takenUpgrades.Count; i++) {
            for (int j = 0; i < availableUpgrades.Count; j++) {
                if (takenUpgrades[i] == availableUpgrades[j] & !Resources.GetMTUpgrade(takenUpgrades[i]).stackable) {
                    availableUpgrades.RemoveAt(j);
                    break;
                }
            }
        }

        var upgrades = new List<Resources.MTUpgradeType>();

        for (int i = availableUpgrades.Count-1; i >= 0; i--) {
            var random = Seed.IntRange(0, availableUpgrades.Count-1);
            upgrades.Add(availableUpgrades[random]);
            availableUpgrades.RemoveAt(random);
            if (upgrades.Count >= 3) {
                this.upgrades = upgrades;
                return;
            }
        }
    }
}