using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour, IActivation
{
    [SerializeField] private QuickInventory qi;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private Arsenal arsenal;
    [SerializeField] private ItemSorter sorter;

    private Dictionary<Resources.Arsenal, InventoryItem> iitems = new Dictionary<Resources.Arsenal, InventoryItem>();
    [HideInInspector] public Option<InventoryItem> currentlyDragged;

    public QuickInventory QuickInventory {
        get { return qi; }
    }

    public void NewInventoryItem(Resources.Arsenal arsenalType) {
        var instance = Instantiate(inventoryItemPrefab, transform, false);
        var iitem = instance.GetComponent<InventoryItem>();
        iitem.CStart(this, arsenalType);
        iitems.Add(arsenalType, iitem);
        sorter.AddItem(iitem.transform);
        if (gameObject.activeInHierarchy) {
            sorter.RevertItemScaling();
            sorter.SortItems();
        }
    }

    public void DStart(Arsenal arsenal)
    {
        foreach (var key in arsenal.Keys)
        {
            if (key.unlocked)
            {
                NewInventoryItem(key.type);
            }
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

    public bool State()
    {
        return gameObject.activeInHierarchy;
    }
}
