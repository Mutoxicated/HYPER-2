using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Arsenal : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private ArsenalItem[] aitems;
    [SerializeField] private ItemSorter sorter;
    [SerializeField] private ArsenalInfo ai;

    public ArsenalItem[] AItems {
        get { return aitems; }
    }

    public void DStart() {
        ai.DStart();
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
            item.gameObject.SetActive(true);
        }
    }

    public void Buy(ArsenalItem arsenalItem) {
        arsenalItem.SetBought(true);
        ai.Unlock();
        ai.Enable(arsenalItem, false);
        inventory.NewInventoryItem(arsenalItem.Type);
    }

    public void Sell(ArsenalItem arsenalItem) {
        arsenalItem.SetBought(false);
        ai.Unlock();
        ai.Enable(arsenalItem, false);
        inventory.RemoveInventoryItem(arsenalItem.Type);
    }

    public void Disable() {
        sorter.RevertItemScaling();
        foreach(var item in aitems) {
            item.gameObject.SetActive(false);
        }
    }
}
