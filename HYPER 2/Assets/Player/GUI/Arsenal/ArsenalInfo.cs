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
    [SerializeField] private UnityEngine.UI.Button buy;
    [SerializeField] private UnityEngine.UI.Button sell;

    private string ufmtName;
    private string ufmtElement;
    private List<string> ufmtStats = new List<string>();
    private string ufmtWorth;

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
        arsenalName.text = String.Format(ufmtName, aitem.name);
        arsenalElement.text = String.Format(ufmtElement, Elm.ToString(current.element));
        for (int i = 0; i < arsenalStats.Count; i++) {
            arsenalStats[i].text = String.Format(ufmtStats[i], "<color=#eeeeee>"+current.values[i]);
        }
        arsenalWorth.text = String.Format(ufmtWorth, "<color=#ee9a00>"+current.worth);
        if (aitem.Type == Resources.Arsenal.FIRST_MT) {
            gameObject.SetActive(true);
            sell.gameObject.SetActive(false);
            buy.gameObject.SetActive(false);
            return;
        }
        if (aitem.Bought) {
            sell.gameObject.SetActive(true);
            buy.gameObject.SetActive(false);
        }else {
            buy.gameObject.SetActive(true);
            sell.gameObject.SetActive(false);
        }
        gameObject.SetActive(true);
    }

    public void Unlock() {
        locked = Option<ArsenalItem>.None();
    }

    public void OnResearch() {

    }

    public void OnBuy() {
        arsenal.Buy(currentItem);
    }

    public void OnSell() {
        arsenal.Sell(currentItem);
    }

    public void Disable() {
        if (locked.IsNone())
            gameObject.SetActive(false);
    }
}
