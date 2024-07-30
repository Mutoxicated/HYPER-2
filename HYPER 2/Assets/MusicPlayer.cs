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

    private void Awake() {
        songs[index].Play();
    }

    public void Pause() {
        songs[index].Pause();
    }

    public void Resume() {
        songs[index].UnPause();
    }

    private float musicT;

    private void Update() {
        if (!songs[index].isPlaying) {
            index = Num.Wrap(index++, 0, songs.Count-1);
            songs[index].Play();
            return;
        }

        songs[index].GetOutputData(samples, 0);
        for (int i = 0; i < samples.Length; i++) {
            avg += samples[i];
        }
        avg /= samples.Length;
        t = Mathf.InverseLerp(-1f, 1f, avg);
        musicT = Mathf.Lerp(musicT, Mathf.InverseLerp(0f, 1f, t-newT), Time.deltaTime*25f);
        MusicVisualization.SetT(musicT);
        avg = 0f;
        newT = t;
    }
}
