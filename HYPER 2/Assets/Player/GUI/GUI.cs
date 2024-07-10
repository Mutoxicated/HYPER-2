using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GUI : MonoBehaviour
{
    [Header("Death UI")]
    [SerializeField] private GameObject deathUI;

    [Header("Playing UI")]
    [SerializeField] private GameObject playingUI;
    [SerializeField] private QuickInventory quickInventory;
    [SerializeField] private Selector selector;

    [Header("Menu UI")]
    [SerializeField] private GameObject menuUI;

    public Color UIColor {
        get { return Save.pData.UIColor; }
    }

    //private Player player;

    public void DStart(Player player) {
        //this.player = player;
        selector.DStart(player, player.Manager);
        quickInventory.DStart();
    }

    public void DeadUI() {
        deathUI.SetActive(true);
        playingUI.SetActive(false);
    }
}
