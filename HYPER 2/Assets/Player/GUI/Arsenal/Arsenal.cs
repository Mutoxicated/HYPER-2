using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arsenal : MonoBehaviour
{
    [SerializeField] private ArsenalItem[] aitems;
    [SerializeField] private ItemSorter sorter;
    [SerializeField] private ArsenalInfo ai;

    public void CStart() {
        ai.CStart();
        sorter.PrepareValues();
    }

    private void OnEnable() {
        sorter.SortItems();
        foreach (var item in aitems) {
            item.gameObject.SetActive(true);
        }
    }

    private void OnDisable() {
        sorter.RevertItemScaling();
        foreach(var item in aitems) {
            item.gameObject.SetActive(false);
        }
    }
}
