using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Arsenal : MonoBehaviour, IActivation
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject arsenalItemPrefab;
    [SerializeField] private Transform arsenalItems;
    [SerializeField] private List<ArsenalItem> aitems;
    [SerializeField] private ItemSorter sorter;
    [SerializeField] private ArsenalInfo ai;
    [SerializeField] private Researcher researcher;

    public Inventory Inventory {
        get { return inventory;}
    }

    public List<ArsenalItem> AItems {
        get { return aitems; }
    }

    public void DStart(Player player) {
        player.onLvlUp += lvl => {
            foreach (var aitem in aitems) {
                if (aitem.MTInfo.unlockAt != lvl) { continue; }
                player.GUI.PushNotif("New minitower unlocked!");
                ai.Unlock();
                ai.Enable(aitem, false);
                inventory.NewInventoryItem(aitem.Type);
                if (gameObject.activeInHierarchy) {
                    Disable();
                    Enable();
                }
            }
        };
        ai.DStart();
        var enumVals = Enum.GetValues(typeof(Resources.Arsenal)) as Resources.Arsenal[];
        foreach (var arsenal in enumVals) {
            var aitem = Instantiate(arsenalItemPrefab, arsenalItems).GetComponent<ArsenalItem>();
            aitem.CStart(ai, arsenal);
            aitems.Add(aitem);
        }
        aitems.Sort();
        foreach (var aitem in aitems) {
            sorter.AddItem(aitem.transform);
        }
        sorter.PrepareValues();
    }

    public void Enable() {
        sorter.SortItems();
        foreach (var item in aitems) {
            item.Enable();
        }
        gameObject.SetActive(true);
    }

    public void OnResearch(ArsenalItem arsenalItem) {
        bool success = researcher.ResearchItem(arsenalItem.Type);
        if (!success) { return; }
        arsenalItem.SetImageColor();
        ai.Enable(arsenalItem, false);
    }

    public void OnStopResearch(ArsenalItem arsenalItem) {
        researcher.StopResearchingItem(arsenalItem.Type);
        arsenalItem.SetImageColor();
        ai.Enable(arsenalItem, false);
    }

    // public void Buy(ArsenalItem arsenalItem) {
    //     if (Save.rData.Money >= arsenalItem.Template.worth) {
    //         Player.SetMoneyRelative(-arsenalItem.Template.worth);
    //     }else {
    //         return;
    //     }
    //     arsenalItem.SetBought(true);
    //     ai.Unlock();
    //     ai.Enable(arsenalItem, false);
    //     inventory.NewInventoryItem(arsenalItem.Type);
    // }

    // public void Sell(ArsenalItem arsenalItem) {
    //     Player.SetMoneyRelative(arsenalItem.Template.worth/2);
    //     arsenalItem.SetBought(false);
    //     ai.Unlock();
    //     ai.Enable(arsenalItem, false);
    //     inventory.RemoveInventoryItem(arsenalItem.Type);
    // }

    public void Disable() {
        sorter.RevertItemScaling();
        ai.Disable(true);
        foreach(var item in aitems) {
            item.gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
    }
}
