using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI : MonoBehaviour
{
    [SerializeField] private GameObject deathUI;
    [SerializeField] private GameObject playingUI;
    [SerializeField] private GameObject menuUI;

    public Color UIColor {
        get { return Save.pData.UIColor; }
    }

    public void DeadUI() {
        deathUI.SetActive(true);
        playingUI.SetActive(true);
    }
}
