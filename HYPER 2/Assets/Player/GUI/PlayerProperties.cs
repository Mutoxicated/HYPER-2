using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProperties : MonoBehaviour
{
    [SerializeField] private Image xpBar; 
    [SerializeField] private TMP_Text money;
    [SerializeField] private TMP_Text lvl;

    private string moneyString;
    private string lvlString;

    private void Start() {
        moneyString = money.text;
        lvlString = lvl.text;
    }

    private void Update() {
        money.text = String.Format(moneyString, Save.rData.Money);
        lvl.text = String.Format(lvlString, Save.rData.Lvl);
        if (Save.rData.XP == 0) {
            xpBar.fillAmount = 0f;
            return;
        }
        //Debug.Log(Save.rData.XP);
        xpBar.fillAmount = Save.rData.XP/Player.MaxXPRequired();
    }
}
