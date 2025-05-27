using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{
    private List<Upgrades> takenUpgrades = new List<Upgrades>();
    private List<Powerups> powerups = new List<Powerups>();

    public void DStart()
    {
        takenUpgrades = Save.rData.TakenUpgrades;
        powerups = Save.rData.Powerups;

        PublicEvent.onSaveRun += _ =>
        {
            Save.rData.TakenUpgrades = takenUpgrades;
            Save.rData.Powerups = powerups;
        };
    }

    public void InitializeArsenalTypeData(Resources.Arsenal arsenal)
    {
        if ((int)arsenal < powerups.Count)
        {
            return;
        }
        powerups.Add(new Powerups());
        takenUpgrades.Add(new Upgrades());
    }

    public void AddPowerup(Resources.Arsenal arsenal, MinitowerInfo mtinfo)
    {
        var index = (int)arsenal;
        powerups[(int)arsenal].self.Add(new Powerup(arsenal, mtinfo, takenUpgrades[(int)arsenal].self));
    }

    public void RemoveLatestPowerup(Resources.Arsenal arsenal)
    {
        powerups[(int)arsenal].self.RemoveAt(0);
    }

    public void AddUpgrade(Resources.Arsenal arsenalType, Resources.MTUpgradeType upgradeType)
    {
        takenUpgrades[(int)arsenalType].self.Add(upgradeType);
    }
}
