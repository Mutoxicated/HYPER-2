using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PowerupMono : MonoBehaviour
{
    [SerializeField] private GameObject innerHolder;
    [SerializeField] private TMP_Text upgradesCount;

    private Resources.Arsenal arsenalType;
    private string ufmtPowerupsCount;
    private int index;

    private int count;

    public void CStart(Resources.Arsenal arsenalType, int index) {
        this.arsenalType = arsenalType;
        ufmtPowerupsCount = upgradesCount.text;
        this.index = index;
    }

    public void Enable() {
        count = Save.rData.Powerups[index].self.Count;
        if (count == 0) {
            Disable();
            return;
        }
        innerHolder.SetActive(true);
    }

    public void Disable() {
        innerHolder.SetActive(false);
    }

    private void Update() {
        count = Save.rData.Powerups[index].self.Count;
        if (count == 0) {
            Disable();
        }
        upgradesCount.text = String.Format(ufmtPowerupsCount, count);
    }
}
