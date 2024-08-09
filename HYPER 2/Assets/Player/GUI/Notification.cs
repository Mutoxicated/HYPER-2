using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Notification : MonoBehaviour
{
    [SerializeField] private Transform notifTransform;
    [SerializeField] private TMP_Text msg;
    [SerializeField] private float duration = 2.5f;
    [SerializeField] private AnimationCurve scaleModifier;

    private List<string> messages = new List<string>();

    private float time = 0f;
    // private float timeInterval = 0f;
    // private float flickerInterval = 0.3f;
    // private int flickerAmount = 2;
    // private int currentFlickerAmount = 0;

    private Vector3 initScale;

    public void CStart() {
        var keys = scaleModifier.keys;
        for (int i = 0; i < keys.Length; i++) {
            keys[i].time *= duration;
        }
        scaleModifier.keys = keys;
        scaleModifier.postWrapMode = WrapMode.ClampForever;

        initScale = notifTransform.localScale;
    }

    public void Enable(string msg) {
        messages.Add(msg);
        if (messages.Count == 1) {
            this.msg.text = msg;
            gameObject.SetActive(true);
        }
    }

    private void Disable() {
        time = 0f;
        gameObject.SetActive(false);
    }

    private void Update() {
        time += Time.deltaTime;

        notifTransform.localScale = new Vector3(initScale.x, initScale.y*scaleModifier.Evaluate(time), initScale.z);

        if (time >= duration) {
            messages.RemoveAt(0);
            if (messages.Count == 0)
                Disable();
            else {
                time = 0f;
                msg.text = messages[0];
            }
        }
    }
}
