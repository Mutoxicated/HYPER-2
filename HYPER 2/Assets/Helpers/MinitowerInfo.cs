using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MTInfo", menuName = "Mutox/MTInfo")]
public class MinitowerInfo : ScriptableObject
{
    public int worth = 1;
    public int unlockAt = 1;

    public bool upgradable = true;
    public List<Resources.MTUpgradeType> availableUpgrades = new List<Resources.MTUpgradeType>();
}