using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using Helpers;
using UnityEngine.UI;

public class ArsenalInfo : MonoBehaviour
{
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

    public void CStart() {
        ufmtName = arsenalName.text;
        ufmtElement = arsenalElement.text;
        for (int i = 0; i < arsenalStats.Count; i++) {
            ufmtStats.Add(arsenalStats[i].text);
        }
        ufmtWorth = arsenalWorth.text;
    }

    public void Enable(ArsenalItem aitem) {
        current = aitem.Template;
        arsenalName.text = String.Format(ufmtName, aitem.name);
        arsenalElement.text = String.Format(ufmtElement, Elm.ToString(current.element));
        for (int i = 0; i < arsenalStats.Count; i++) {
            arsenalStats[i].text = String.Format(ufmtStats[i], "<color=#eeeeee>"+current.values[i]);
        }
        arsenalWorth.text = String.Format(ufmtWorth, "<color=#ee9a00>"+current.worth);
        if (aitem.Bought) {
            sell.gameObject.SetActive(true);
            buy.gameObject.SetActive(true);
        }else {
            buy.gameObject.SetActive(true);
            sell.gameObject.SetActive(true);
        }
        gameObject.SetActive(true);
    }

    public void Lock(ArsenalItem aitem) {
        locked = new Option<ArsenalItem>(aitem);
    }

    public void Unlock() {
        locked = Option<ArsenalItem>.None();
    }

    public void Disable() {
        if (locked.IsNone())
            gameObject.SetActive(false);
    }
}
