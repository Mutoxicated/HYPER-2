using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sfx : MonoBehaviour
{
    [SerializeField] private AudioSource sound;

    public void Awake() {
        sound.volume = Save.pData.SFXVolume;
    }
}
