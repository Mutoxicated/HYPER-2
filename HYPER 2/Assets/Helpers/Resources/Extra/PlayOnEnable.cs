using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayOnEnable : MonoBehaviour
{
    [SerializeField] private ParticleSystem ps;
    [SerializeField] private Option<AudioSource> audioSource;

    private void OnEnable() {
        Debug.Log("enabled at "+gameObject.name, gameObject);
        ps.Play();
        audioSource.GetAndThen(a => {
            a.Play();
        });
    }

    private void Update() {
        if (ps.isStopped) {
            Destroy(gameObject);
        }
    }
}
