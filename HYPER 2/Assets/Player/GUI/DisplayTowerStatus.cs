using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayTowerStatus : MonoBehaviour
{
    [SerializeField] private Stats towerStats;
    [Header("UI Elements")]
    [SerializeField] private TMP_Text currentHP;
    [SerializeField] private Image hpBar;

    private string ufmtCurrent;

    public void CStart() {
        ufmtCurrent = currentHP.text;
        gameObject.SetActive(true);
    }

    private void Update() {
        currentHP.text = String.Format(ufmtCurrent, towerStats.HP, towerStats.get(Stat.MAX_HEALTH));
        hpBar.fillAmount = towerStats.HP/towerStats.get(Stat.MAX_HEALTH);
    }
}
