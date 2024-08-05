using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Audio {
    BOOP,
    BEEP,
}

public class SoundCollection : MonoBehaviour
{
    private static SoundCollection self;

    [SerializeField] private AudioClip[] audioClips;
    private static Dictionary<Audio, AudioSource> audios = new Dictionary<Audio, AudioSource>();

    private void Start() {
        audios.Clear();
        self = this;
        foreach (var clip in audioClips) {
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = clip;
            if (clip.name == "boop") {
                audios.Add(Audio.BOOP, audioSource);
            }else if (clip.name == "beep") {
                audios.Add(Audio.BEEP, audioSource);
            }
        }
    }

    public static void Play(Audio audio) {
        audios[audio].volume = Save.pData.SFXVolume;
        audios[audio].Play();
    }
}
