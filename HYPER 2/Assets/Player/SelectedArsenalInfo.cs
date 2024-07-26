using System;
using System.Collections;
using System.Collections.Generic;
using Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedArsenalInfo : MonoBehaviour
{
    [SerializeField] private Selector selector;
    [SerializeField] private TMP_Text arsenalName;
    [SerializeField] private TMP_Text arsenalElement;
    [SerializeField] private List<TMP_Text> arsenalStats = new List<TMP_Text>();
    [SerializeField] private TMP_Text hpCurrentMax;
    [SerializeField] private Image hpBar;
    [SerializeField] private TMP_Text xpCurrentMax;
    [SerializeField] private Image xpBar;
    [SerializeField] private TMP_Text hpRestoreCost;
    [SerializeField] private TMP_Text lvl;
    private Option<MiniTower> mt = Option<MiniTower>.None();
    private Stats stats;

    private string ufmtName;
    private string ufmtElement;
    private List<string> ufmtStats = new List<string>();
    private string ufmtCurrentMax;
    private string ufmtXPCurrentMax;
    private string ufmtRestoreCost;
    private string ufmtLvl;

    private Gameplay gamePlay;

    public void DStart(Gameplay gameplay) {
        this.gamePlay = gameplay;
        ufmtName = arsenalName.text;
        ufmtElement = arsenalElement.text;
        for (int i = 0; i < arsenalStats.Count; i++) {
            ufmtStats.Add(arsenalStats[i].text);
        }
        ufmtCurrentMax = hpCurrentMax.text;
        ufmtXPCurrentMax = xpCurrentMax.text;
        ufmtRestoreCost = hpRestoreCost.text;
        ufmtLvl = lvl.text;
    }

    public void Enable(MiniTower mt) {
        this.mt = new Option<MiniTower>(mt);
        stats = mt.Stats;
        gameObject.SetActive(true);
    }

    public void Update() {
        mt.GetAndThen(m => {
            arsenalName.text = String.Format(ufmtName, Arsnl.ToName(m.ArsenalType));
            arsenalElement.text = String.Format(ufmtElement, Elm.ToString(stats.StatElement));
            for (int i = 0; i < arsenalStats.Count; i++) {
                arsenalStats[i].text = String.Format(ufmtStats[i], "<color=#eeeeee>"+stats.Template.statsBase.values[i]+"<color=#999999> * "+m.Stats.modify(Stt.EnumValues[i]));
            }
            hpCurrentMax.text = String.Format(ufmtCurrentMax, Mathf.RoundToInt(m.HP), stats.get(Stat.MAX_HEALTH));
            hpBar.fillAmount = m.HP/stats.get(Stat.MAX_HEALTH);
            xpCurrentMax.text = String.Format(ufmtXPCurrentMax, m.Stats.XP, m.Stats.Template.maxXp);
            xpBar.fillAmount =  (float)m.Stats.XP/(float)m.Stats.Template.maxXp;
            lvl.text = String.Format(ufmtLvl, m.Stats.LVL);

            hpRestoreCost.text = String.Format(ufmtRestoreCost, Mathf.RoundToInt(gamePlay.LinearT*15)+"*");
            gameObject.SetActive(true);
        });
    }

    public void Disable() {
        selector.OnDeselect();
        gameObject.SetActive(false);
        mt = Option<MiniTower>.None();
    }

    public void OnRestoreHP() {
        if (Save.rData.Money < Mathf.RoundToInt(gamePlay.LinearT*15)) {
            return;
        }
        Player.SetMoneyRelative(-Mathf.RoundToInt(gamePlay.LinearT*15));
        mt.Get().RestoreHP();
    }

    public void Destroy() {
        selector.Delete();
        mt = Option<MiniTower>.None();
        Disable();
    }
}
