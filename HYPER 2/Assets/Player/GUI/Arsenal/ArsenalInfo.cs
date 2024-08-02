using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using Helpers;
using UnityEngine.UI;

public class ArsenalInfo : MonoBehaviour
{
    [SerializeField] private Arsenal arsenal;
    [SerializeField] private TMP_Text arsenalName;
    [SerializeField] private TMP_Text arsenalElement;
    [SerializeField] private List<TMP_Text> arsenalStats = new List<TMP_Text>();
    [SerializeField] private TMP_Text arsenalWorth;
    [SerializeField] private Image researchBar;
    [SerializeField] private TMP_Text researchLvl;
    [SerializeField] private UnityEngine.UI.Button research;
    [SerializeField] private UnityEngine.UI.Button stopResearch;
    // [SerializeField] private UnityEngine.UI.Button buy;
    // [SerializeField] private UnityEngine.UI.Button sell;

    public Arsenal Arsenal {
        get { return arsenal; }
    }

    private string ufmtName;
    private string ufmtElement;
    private List<string> ufmtStats = new List<string>();
    private string ufmtWorth;
    private string ufmtResearchLvl;

    private StatsTemplate current;
    private Option<ArsenalItem> locked = Option<ArsenalItem>.None();
    private ArsenalItem currentItem;

    public void DStart() {
        ufmtName = arsenalName.text;
        ufmtElement = arsenalElement.text;
        for (int i = 0; i < arsenalStats.Count; i++) {
            ufmtStats.Add(arsenalStats[i].text);
        }
        ufmtWorth = arsenalWorth.text;
        ufmtResearchLvl = researchLvl.text;
    }

    public void Enable(ArsenalItem aitem, bool mustLock) {
        if (locked.IsSome() && !mustLock) {
            return;
        }
        if (mustLock) {
            locked = new Option<ArsenalItem>(aitem);
        }

        currentItem = aitem;
        current = aitem.Template;
        if (aitem.Locked) {
            researchBar.transform.parent.gameObject.SetActive(false);
            researchLvl.gameObject.SetActive(false);
            research.gameObject.SetActive(false);
            stopResearch.gameObject.SetActive(false);
            arsenalName.text = "???";
            arsenalElement.text = "Unlock at lvl "+current.unlockAt;
            for (int i = 0; i < arsenalStats.Count; i++) {
                arsenalStats[i].text = "";
            }
            arsenalWorth.text = "";
            gameObject.SetActive(true);
            return;
        }
        arsenalName.text = String.Format(ufmtName, Arsnl.ToName(aitem.Type));
        arsenalElement.text = String.Format(ufmtElement, Elm.ToString(current.element));
        for (int i = 0; i < arsenalStats.Count; i++) {
            arsenalStats[i].text = String.Format(ufmtStats[i], "<color=#eeeeee>"+Mathf.RoundToInt(current.statsBase.values[i]*current.modifiers[i]));
        }
        arsenalWorth.text = String.Format(ufmtWorth, "<color=#ee9a00>"+current.worth);
        if (aitem.BeingResearched) {
            researchBar.transform.parent.gameObject.SetActive(true);
            researchLvl.gameObject.SetActive(true);
            research.gameObject.SetActive(false);
            stopResearch.gameObject.SetActive(true);
        }else {
            researchBar.transform.parent.gameObject.SetActive(false);
            researchLvl.gameObject.SetActive(false);
            research.gameObject.SetActive(true);
            stopResearch.gameObject.SetActive(false);
        }
        if (aitem.Type == Resources.Arsenal.FIRST_MT) {
            gameObject.SetActive(true);
            //sell.gameObject.SetActive(false);
            //buy.gameObject.SetActive(false);
            return;
        }
        // if (aitem.Bought) {
        //     sell.gameObject.SetActive(true);
        //     buy.gameObject.SetActive(false);
        // }else {
        //     buy.gameObject.SetActive(true);
        //     sell.gameObject.SetActive(false);
        // }
        gameObject.SetActive(true);
    }

    public void Unlock() {
        locked = Option<ArsenalItem>.None();
    }

    public void OnResearch() {
        arsenal.OnResearch(currentItem);
    }

      public void OnStopResearch() {
        arsenal.OnStopResearch(currentItem);
    }

    // public void OnBuy() {
    //     arsenal.Buy(currentItem);
    // }

    // public void OnSell() {
    //     arsenal.Sell(currentItem);
    // }

    public void Disable(bool must = false) {
        if (must) {
            locked = Option<ArsenalItem>.None();
            gameObject.SetActive(false);
        }
        if (locked.IsNone())
            gameObject.SetActive(false);
    }

    public void Update() {
        var filled = Num.MinLimit(Save.rData[currentItem.Type], 0f);
        researchBar.fillAmount = filled/currentItem.Template.maxXp;
        researchLvl.text = String.Format(ufmtResearchLvl, Save.rData[currentItem.Type, true]);
    }
}
