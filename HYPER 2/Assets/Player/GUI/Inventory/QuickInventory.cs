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

    private QuickInventoryItem selected;
    private float imageSize;
    private float length;

    public Message AddNewItem(object obj) {
        throw new NotImplementedException();
    }

    public Option<Resources.Arsenal> GetSelectedArsenal() {

        return selected.ArsenalType;
    }

    public void UpdateSelected(QuickInventoryItem item) {
        selected = item;
        selectedImage.position = item.transform.position;
        Save.rData.SetSelectedIndex(item.Index);
    }

    public void DStart(){
        imageSize = items[0].transform.localScale.x*0.1f+spacing;
        
        Sort();
        edit.position = items[capacity-1].transform.position+Vector3.right*imageSize;
        selected = items[0];
        selected.transform.position = items[0].transform.position;

        Save.rData.UpdateQIACapacity(capacity);
        int index = 0;
        foreach (var qia in Save.rData.QIAs) {
            items[index].SetArsenal(qia);
            index++;
        }
        UpdateSelected(items[Save.rData.SelectedIndex]);
        gameObject.SetActive(true);
    }

    private void Update() {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) {
            UpdateSelectedWithIndex(0);
        }else if (Keyboard.current.digit2Key.wasPressedThisFrame) {
            UpdateSelectedWithIndex(1);
        }else if (Keyboard.current.digit3Key.wasPressedThisFrame) {
            UpdateSelectedWithIndex(2);
        }else if (Keyboard.current.digit4Key.wasPressedThisFrame) {
            UpdateSelectedWithIndex(3);
        }else if (Keyboard.current.digit5Key.wasPressedThisFrame) {
            UpdateSelectedWithIndex(4);
        }else if (Keyboard.current.digit6Key.wasPressedThisFrame) {
            UpdateSelectedWithIndex(5);
        }
    }

    private void UpdateSelectedWithIndex(int index) {
        if (index >= capacity) {
            index = capacity-1;
        }
        UpdateSelected(items[index]);
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