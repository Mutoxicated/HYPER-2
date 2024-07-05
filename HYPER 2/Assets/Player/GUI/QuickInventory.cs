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
    [SerializeField] private GameObject selectObj;
    [SerializeField] private int capacity = 3;
    [SerializeField] private List<GameObject> slots = new List<GameObject>();
    [SerializeField] private List<Image> images = new List<Image>();

    private float imageSize;
    private float length;

    public Message AddNewItem(object obj) {
        throw new NotImplementedException();
    }

    public void DStart(){
        imageSize = slots[0].transform.localScale.x*0.1f+spacing;
        
        Sort();
        UpdateSelected();
        gameObject.SetActive(true);
    }

    private void Sort() {
        length = imageSize*capacity;

        for (int i = 0; i < capacity; i++) {
            slots[i].gameObject.SetActive(true);
        }
        for (int i = capacity; i < slots.Count; i++) {
            slots[i].gameObject.SetActive(false);
        }

        var step = length/(capacity-1);

        var index = 0;
        for (float i = -length/2; i <= length/2; i += step) {
            slots[index].transform.position = transform.position+Vector3.right*i;
            index++;
        }
    }

    private void UpdateSelected(){
        selectObj.transform.position = slots[0].transform.position;
    }

    // private void Update(){
    //     UpdateImages();
    //     UpdateSelected();
    // }
}