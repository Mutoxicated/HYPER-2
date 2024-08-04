using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScreen : MonoBehaviour
{
    private float time = 0f;
    private float duration = 1.8f;

    public void Enable() {
        gameObject.SetActive(true);
    }

    public void Update() {
        if (time >= duration) {
            return;   
        }
        time += Time.unscaledDeltaTime;
        Time.timeScale = Mathf.Pow(Mathf.InverseLerp(duration, 0f, time), 2);
    }
}
