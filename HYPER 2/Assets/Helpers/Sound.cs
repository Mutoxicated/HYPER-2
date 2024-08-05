using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    [SerializeField] private AudioSource sound;

    public void Play() {
        transform.parent = null;
        gameObject.SetActive(true);
        sound.volume = Save.pData.SFXVolume;
        sound.Play();
    }

    public void Update() {
        if (!sound.isPlaying) {
            Destroy(gameObject);
            return;
        }
    }
}
