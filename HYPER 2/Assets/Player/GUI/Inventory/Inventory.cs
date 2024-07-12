using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private Arsenal arsenal;
    [SerializeField] private ItemSorter sorter;

    private Dictionary<Resources.Arsenal, InventoryItem> iitems = new Dictionary<Resources.Arsenal, InventoryItem>();

    public void NewInventoryItem(Resources.Arsenal arsenalType) {
        var instance = Instantiate(inventoryItemPrefab, transform, false);
        var iitem = instance.GetComponent<InventoryItem>();
        iitem.CStart(arsenalType);
        iitems.Add(arsenalType, iitem);
        Save.rData.AddBoughtArsenal(arsenalType);
        sorter.AddItem(iitem.transform);
    }

    public void RemoveInventoryItem(Resources.Arsenal arsenalType) {
        sorter.RemoveItem(iitems[arsenalType].transform);
        Destroy(iitems[arsenalType].gameObject);
        Save.rData.RemoveBoughtArsenal(arsenalType);
    }

    public void DStart() {
        foreach (var bought in Save.rData.BoughtArsenals) {
            var iitem = Instantiate(inventoryItemPrefab, transform, false).GetComponent<InventoryItem>();
            iitem.CStart(bought);
            iitems.Add(bought, iitem);
            sorter.AddItem(iitem.transform);
        }
        sorter.PrepareValues();
    }

    public void Enable() {
        sorter.SortItems();
        gameObject.SetActive(true);
    }

    public void Disable() {
        sorter.RevertItemScaling();
        gameObject.SetActive(false);
    }
}
