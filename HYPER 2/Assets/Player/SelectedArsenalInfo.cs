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
    private Option<MiniTower> mt = Option<MiniTower>.None();
    private Stats stats;

    private string ufmtName;
    private string ufmtElement;
    private List<string> ufmtStats = new List<string>();
    private string ufmtCurrentMax;
    private bool locked = false;

    public bool Locked {
        get { return locked; }
    }

    public void DStart() {
        ufmtName = arsenalName.text;
        ufmtElement = arsenalElement.text;
        for (int i = 0; i < arsenalStats.Count; i++) {
            ufmtStats.Add(arsenalStats[i].text);
        }
        ufmtCurrentMax = hpCurrentMax.text;
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
                arsenalStats[i].text = String.Format(ufmtStats[i], "<color=#eeeeee>"+stats.Template.values[i]+"<color=#999999> * "+stats.mod[stats.Template.stats[i]]);
            }
            hpCurrentMax.text = String.Format(ufmtCurrentMax, Mathf.RoundToInt(m.HP.Current), m.HP.Max);
            hpBar.fillAmount = m.HP.Current/m.HP.Max;
            gameObject.SetActive(true);
        });
    }

    public void Lock() {
        locked = true;
    }

    public void Disable(bool must) {
        if (!must && locked) { return; }
        selector.OnDeselect();
        gameObject.SetActive(false);
        mt = Option<MiniTower>.None();
        locked = false;
    }

    public void OnRestoreHP() {
        if (Save.rData.Money < Gameplay.linearT*15) {
            return;
        }
        Player.SetMoneyRelative(-Mathf.RoundToInt(Gameplay.linearT*15));
        mt.Get().RestoreHP();
    }

    public void Destroy() {
        selector.Delete(mt.Get().gameObject);
        mt = Option<MiniTower>.None();
        Disable(true);
    }
}
