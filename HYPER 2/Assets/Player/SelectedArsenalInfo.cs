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
        if (this.mt.IsSome()) {
            Disable();
        }
        this.mt = new Option<MiniTower>(mt);
        mt.Highlight();
        mt.AddOnDeath(_ => Disable());
        selector.OnSelect(mt.gameObject);
        stats = mt.Stats;
        gameObject.SetActive(true);
    }

    public void Update() {
        if (mt.Get() == null) {
            Disable();
            return;
        }
        mt.GetAndThen(m => {
            arsenalName.text = String.Format(ufmtName, Arsnl.ToName(m.ArsenalType));
            arsenalElement.text = String.Format(ufmtElement, Elm.ToString(stats.StatElement));
            for (int i = 0; i < arsenalStats.Count; i++) {
                arsenalStats[i].text = String.Format(ufmtStats[i], "<color=#eeeeee>"+stats.Template.statsBase.values[i]+"<color=#999999> * "+m.Stats.modify(Stt.EnumValues[i]));
            }
            hpCurrentMax.text = String.Format(ufmtCurrentMax, Mathf.RoundToInt(m.HP), Mathf.RoundToInt(stats[Stat.MAX_HEALTH]));
            hpBar.fillAmount = m.HP/stats[Stat.MAX_HEALTH];
            xpCurrentMax.text = String.Format(ufmtXPCurrentMax, m.Stats.XP, m.Stats.MaxRequiredXP());
            xpBar.fillAmount =  (float)m.Stats.XP/(float)m.Stats.MaxRequiredXP();
            lvl.text = String.Format(ufmtLvl, m.Stats.LVL);

            hpRestoreCost.text = String.Format(ufmtRestoreCost, Mathf.RoundToInt(gamePlay.RestoreHPPrice)+"*");
            gameObject.SetActive(true);
        });
    }

    public void Disable() {
        if (this == null) { return; }
        if (!gameObject.scene.isLoaded) { return; }
        mt.GetAndThen(m => {
            m.Dehighlight(); 
            m.AddOnDeath(_ => Disable());
        });
        selector.OnDeselect();
        gameObject.SetActive(false);
        mt = Option<MiniTower>.None();
    }

    public void OnRestoreHP() {
        if (Save.rData.Money < Mathf.RoundToInt(gamePlay.RestoreHPPrice)) {
            return;
        }
        Player.self.SetMoneyRelative(-Mathf.RoundToInt(gamePlay.RestoreHPPrice));
        mt.Get().RestoreHP();
    }

    public void Destroy() {
        Player.self.SetMoneyRelative(mt.Get().MTInfo.worth/2);
        selector.Delete();
        mt = Option<MiniTower>.None();
    }
}
