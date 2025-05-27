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
        money.text = String.Format(moneyString, Player.self.Money);
        lvl.text = String.Format(lvlString, Player.self.Lvl);
        if (Player.self.CurrentXP == 0) {
            xpBar.fillAmount = 0f;
            return;
        }
        //Debug.Log(Save.rData.XP);
        xpBar.fillAmount = Player.self.CurrentXP/Player.MaxXPRequired();
    }
}
