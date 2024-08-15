using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using TMPro;
using UnityEngine;

public class PowerupPopup : MonoBehaviour
{
    [SerializeField] private Upgrade[] upgrades;
    [SerializeField] private TMP_Text header;
    [SerializeField] private TMP_Text description;

    private string ufmtDescription;
    private Resources.Arsenal arsenalType;
    private int index;

    public void DStart() {
        ufmtDescription = description.text;
        foreach (var upgrade in upgrades) {
            upgrade.CStart(this);
        }
    }

    public void Enable(int index, Resources.Arsenal arsenalType) {
        this.index = index;
        this.arsenalType = arsenalType;

        for (int i = 0; i < upgrades.Length; i++) {
            upgrades[i].Enable(Save.rData.Powerups[index].self[0].upgrades[i], Save.rData.Powerups[index].self[0].rarities[i]);
        }
        gameObject.SetActive(true);
    }

    public void Disable() {
        gameObject.SetActive(false);
    }

    public void UpgradeTaken(Resources.MTUpgradeType upgradeType) {
        Save.rData.AddTakenUpgrade(arsenalType, upgradeType);
        Save.rData.Powerups[index].self.RemoveAt(0);
        Disable();
    }

    public void ShowDescription(string desc, string rarity) {
        description.text = String.Format(ufmtDescription, desc, rarity);
    }
}
