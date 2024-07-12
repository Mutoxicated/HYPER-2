using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Arsenal arsenal;
    [SerializeField] private ItemSorter sorter;

    private List<ArsenalItem> boughtItems = new List<ArsenalItem>();

    private void Enable() {
        foreach (var item in arsenal.AItems) {
            if (item.Bought) {
                boughtItems.Add(item);
                sorter.AddItem(item.transform);
            }
        }

        sorter.SortItems();
    }

    private void Disable() {
        sorter.RevertItemScaling();
    }
}
