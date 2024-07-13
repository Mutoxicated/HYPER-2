using System;
using System.Collections;
using System.Collections.Generic;
using Helpers;
using TMPro;
using UnityEngine;

public class SelectedArsenalInfo : MonoBehaviour
{
    [SerializeField] private Selector selector;
    [SerializeField] private TMP_Text arsenalName;
    [SerializeField] private TMP_Text arsenalElement;
    [SerializeField] private List<TMP_Text> arsenalStats = new List<TMP_Text>();
    private MiniTower mt;
    private StatsTemplate template;

    private string ufmtName;
    private string ufmtElement;
    private List<string> ufmtStats = new List<string>();

    public void DStart() {
        ufmtName = arsenalName.text;
        ufmtElement = arsenalElement.text;
        for (int i = 0; i < arsenalStats.Count; i++) {
            ufmtStats.Add(arsenalStats[i].text);
        }
    }

    public void Enable(MiniTower mt) {
        this.mt = mt;
        var arsenal = mt.ArsenalType;
        template = Resources.GetStatTemplate(arsenal);
        arsenalName.text = String.Format(ufmtName, Arsnl.ToName(arsenal));
        arsenalElement.text = String.Format(ufmtElement, Elm.ToString(template.element));
        for (int i = 0; i < arsenalStats.Count; i++) {
            arsenalStats[i].text = String.Format(ufmtStats[i], "<color=#eeeeee>"+template.values[i]);
        }
        gameObject.SetActive(true);
    }

    public void Disable() {
        gameObject.SetActive(false);
    }

    public void OnRestoreHP() {
        mt.RestoreHP();
    }

    public void OnDestroy() {
        selector.Delete();
        Disable();
    }
}
