using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI : MonoBehaviour
{
    [Header("Death UI")]
    [SerializeField] private GameObject deathUI;

    [Header("Playing UI")]
    [SerializeField] private GameObject playingUI;
    [SerializeField] private QuickInventory quickInventory;

    [Header("Menu UI")]
    [SerializeField] private GameObject menuUI;


    public Color UIColor {
        get { return Save.pData.UIColor; }
    }

    public void DStart() {
        quickInventory.CStart();
    }

    public void DeadUI() {
        deathUI.SetActive(true);
        playingUI.SetActive(true);
    }
}
