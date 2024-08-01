using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notification : MonoBehaviour
{
    [SerializeField] private Transform notifTransform;

    [SerializeField] private float duration = 2.5f;
    [SerializeField] private float endScaleX = 2f;
    [SerializeField] private AnimationCurve scaleModifier;

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

    public void Enable() {
        gameObject.SetActive(true);
    }

    private void Disable() {
        time = 0f;
        gameObject.SetActive(false);
    }

    private void Update() {
        time += Time.deltaTime;

        notifTransform.localScale = initScale+Vector3.right*endScaleX*scaleModifier.Evaluate(time);

        if (time >= duration) {
            Disable();
        }
    }
}
