using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private List<AudioSource> songs;

    private int index = 0;

    // music visualization related
    private float[] samples = new float[4] {
        0f,0f,0f,0f
    };
    private float avg;

    private void Awake() {
        songs[index].Play();
    }

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
        MusicVisualization.SetT(Mathf.Lerp(-1, 1, avg/samples.Length));
        avg = 0f;
    }
}
