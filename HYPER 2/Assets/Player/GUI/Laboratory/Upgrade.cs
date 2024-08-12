using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    [SerializeField] private GameObject innerHolder;
    [SerializeField] private TMP_Text upgradesCount;

    private Resources.Arsenal arsenalType;
    private string ufmtUpgradesCount;
    private int index;

    public void CStart(Resources.Arsenal arsenalType, int index) {
        this.arsenalType = arsenalType;
        ufmtUpgradesCount = upgradesCount.text;
    }

    public void Enable() {
        if (!Save.rData[arsenalType, 0]) {
            innerHolder.SetActive(false);
            return;
        }
        innerHolder.SetActive(true);
    }

    public void Disable() {
        innerHolder.SetActive(false);
    }

    private int count;
    private void Update() {
        count = Save.rData.Powerups[index].Count;
        if (count == 0) {
            Disable();
        }
        upgradesCount.text = String.Format(ufmtUpgradesCount, count);
    }
}
