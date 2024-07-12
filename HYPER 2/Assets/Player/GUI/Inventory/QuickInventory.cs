using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class QuickInventory : MonoBehaviour
{   
    public enum Message {
        SUCCESS,
        MAX_REACHED,
        NOT_FOUND
    }

    private const int maxCapacity = 6;
    private const float spacing = 0.01f;

    [SerializeField] private Sprite empty;
    [SerializeField] private Option<QuickInventoryItem> selected;
    [SerializeField] private int capacity = 3;
    [SerializeField] private List<QuickInventoryItem> items = new List<QuickInventoryItem>();

    private float imageSize;
    private float length;

    public Message AddNewItem(object obj) {
        throw new NotImplementedException();
    }

    public Option<Resources.Arsenal> GetSelectedArsenal() {
        if (selected.IsNone()) {
            return Option<Resources.Arsenal>.None();
        }

        return selected.Get().ArsenalType;
    }

    public void UpdateSelected(QuickInventoryItem item) {
        selected = new Option<QuickInventoryItem>(item);
    }

    public void DStart(){
        imageSize = items[0].transform.localScale.x*0.1f+spacing;
        
        Sort();
        selected = new Option<QuickInventoryItem>(items[0]);
        selected.Get().transform.position = items[0].transform.position;
        gameObject.SetActive(true);
    }

    private void Sort() {
        length = imageSize*capacity;

        for (int i = 0; i < capacity; i++) {
            items[i].CStart(i);
        }
        // for (int i = capacity; i < items.Count; i++) {
        //     items[i].gameObject.SetActive(false);
        // }

        var step = length/(capacity-1);

        var index = 0;
        for (float i = -length/2; i <= length/2; i += step) {
            items[index].transform.position = transform.position+Vector3.right*i;
            index++;
        }
    }

    // private void Update(){
    //     UpdateImages();
    //     UpdateSelected();
    // }
}