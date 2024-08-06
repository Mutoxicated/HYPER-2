using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
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

    [SerializeField] private Transform edit;
    [SerializeField] private Transform selectedImage;
    [SerializeField] private int capacity = 3;
    [SerializeField] private List<QuickInventoryItem> items = new List<QuickInventoryItem>();

    public List<QuickInventoryItem> Items {
        get { return items; }
    }
    public int Capacity {
        get { return capacity; }
    }

    private Option<QuickInventoryItem> selected = Option<QuickInventoryItem>.None();
    private float imageSize;
    private float length;

    private Player player;

    public Option<Resources.Arsenal> GetSelectedArsenal() {
        if (selected.IsNone()) {
            return Option<Resources.Arsenal>.None();
        }
        return selected.Get().ArsenalType;
    }

    public void UpdateSelected(Option<QuickInventoryItem> item) {
        selected = item;
        selected.GetAndThen(i => {
            selectedImage.position = i.transform.position;
            Save.rData.SetSelectedIndex(i.Index);
        }).Else(() => {
            selectedImage.position = new Vector3(5700,0,0);
            Save.rData.SetSelectedIndex(-1);
        });
    }

    public void CheckCDs() {
        for (int i = 0; i < capacity; i++) {
            items[i].CheckCD();
        }
    }

    public void DStart(Player player){
        this.player = player;
        imageSize = items[0].transform.localScale.x*0.2f+spacing;
        
        Sort();
        edit.position = items[capacity-1].transform.position+Vector3.left*imageSize;
        if (Save.rData.SelectedIndex < 0) {
            UpdateSelected(Option<QuickInventoryItem>.None());
        }else {
            UpdateSelectedWithIndex(Save.rData.SelectedIndex);
        }

        Save.rData.UpdateQIACapacity(capacity);
        int index = 0;
        foreach (var qia in Save.rData.QIAs.ToArray()) {
            items[index].SetArsenal(qia);
            index++;
        }
     
        gameObject.SetActive(true);
    }

    public void UpdateQI(int index, Resources.Arsenal arsenal) {
        for (int i = 0; i < capacity; i++) {
            if (i == index) {
                items[index].SetArsenal(new Option<Resources.Arsenal>(arsenal));
                continue;
            }
            if (items[i].ArsenalType.IsNone()) {
                continue;
            }
            if (items[i].ArsenalType.Get() == arsenal) {
                items[i].SetArsenal(Option<Resources.Arsenal>.None());
            }
        }
    }

    private void Update() {
        
        if (player.OnUIDelete) {
            selected.GetAndThen(i => {
                i.SetArsenal(Option<Resources.Arsenal>.None());
            });
        }
        if (Keyboard.current.digit1Key.wasPressedThisFrame) {
            UpdateSelectedWithIndex(4);
        }else if (Keyboard.current.digit2Key.wasPressedThisFrame) {
            UpdateSelectedWithIndex(3);
        }else if (Keyboard.current.digit3Key.wasPressedThisFrame) {
            UpdateSelectedWithIndex(2);
        }else if (Keyboard.current.digit4Key.wasPressedThisFrame) {
            UpdateSelectedWithIndex(1);
        }else if (Keyboard.current.digit5Key.wasPressedThisFrame) {
            UpdateSelectedWithIndex(0);
        }
    }

    public void UpdateSelectedWithIndex(int index) {
        if (index > capacity-1) {
            index = capacity-1;
        }
        selected.GetAndThen(i => {
            if (i.Index == index) {
                UpdateSelected(Option<QuickInventoryItem>.None());
            }else {
                UpdateSelected(new Option<QuickInventoryItem>(items[index]));
            }
        }).Else(() => {
            UpdateSelected(new Option<QuickInventoryItem>(items[index]));
        });
    }

    private void Sort() {
        length = imageSize*capacity;

        for (int i = 0; i < capacity; i++) {
            items[i].CStart(this, i);
        }
        // for (int i = capacity; i < items.Count; i++) {
        //     items[i].gameObject.SetActive(false);
        // }

        var step = length/(capacity-1);

        for (int i = 0; i < capacity; i++) {
            items[i].transform.position = transform.position+Vector3.left*i*step;
        }
    }

    // private void Update(){
    //     UpdateImages();
    //     UpdateSelected();
    // }
}