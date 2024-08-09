using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private List<AudioSource> songs;

    private int index = 0;

    // music visualization related
    private float[] samples = new float[8] {
        0f,0f,0f,0f,0f,0f,0f,0f
    };
    private float avg;
    private float t;
    private float newT;

    private float room = 0.05f;
    private float time = 0.07f;

    private float musicT;
    private bool disabled = false;

    public void Refresh() {
        songs[index].volume = Save.pData.MusicVolume;
    }

    public void Disable() {
        disabled = true;
        songs[index].Pause();
    }

    public void Enable() {
        disabled = false;
        songs[index].UnPause();
    }

    public void DStart() {
        songs[index].Play();
    }

    private void Update() {
        if (disabled) { return; }
        if (!songs[index].isPlaying) {
            index = Num.Wrap(++index, 0, songs.Count-1);
            songs[index].volume = Save.pData.MusicVolume;
            songs[index].Play();
        }

        time += Time.deltaTime;

        songs[index].GetOutputData(samples, 0);
        for (int i = 0; i < samples.Length; i++) {
            avg += samples[i];
        }
        avg /= samples.Length;
        t = Mathf.InverseLerp(-1f, 1f, avg);
        newT = Mathf.Lerp(newT, 0f, Time.deltaTime*0.8f);    

        musicT = Mathf.Lerp(musicT, Mathf.InverseLerp(0f, 1f, t-newT), Time.deltaTime*25f);
        MusicVisualization.SetT(musicT);
        avg = 0f;
        if (time >= room){
            newT = t;
            time = 0f;
        }
    }
}
