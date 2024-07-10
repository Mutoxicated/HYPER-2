using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class decal : MonoBehaviour
{
    [SerializeField] private ParticleSystem ps;

    private ParticleSystem.MainModule main;

    private void Start() {
        main = ps.main;
    }

    private void Update() {
        var lifetime = main.startLifetime;
        lifetime.constant += 1f;
        main.startLifetime = lifetime;
        //main.duration += Time.deltaTime;
    }
}
