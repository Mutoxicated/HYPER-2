using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Arsenal : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject arsenalItemPrefab;
    [SerializeField] private Transform arsenalItems;
    [SerializeField] private List<ArsenalItem> aitems;
    [SerializeField] private ItemSorter sorter;
    [SerializeField] private ArsenalInfo ai;

    public List<ArsenalItem> AItems {
        get { return aitems; }
    }

    public void DStart(Player player) {
        player.onLvlUp += lvl => {
            foreach (var aitem in aitems) {
                if (aitem.Bought) { continue; }
                if (aitem.Template.unlockAt != lvl) { continue; }
                aitem.SetBought(true);
                ai.Unlock();
                ai.Enable(aitem, false);
                inventory.NewInventoryItem(aitem.Type);
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

    private void OnEnable() {
        sorter.SortItems();
        List<Resources.Arsenal> boughtArsenals = Save.rData.BoughtArsenals.ToList();
        foreach (var item in aitems) {
            foreach (var bought in boughtArsenals) {
                if (bought == item.Type) {
                    item.SetBought(true);
                    boughtArsenals.Remove(bought);
                    break;
                }
            }
            item.Enable();
        }
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
        foreach(var item in aitems) {
            item.gameObject.SetActive(false);
        }
    }
}
