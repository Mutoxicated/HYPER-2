using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private Transform envCamera;
    private List<float> strengths = new List<float>(4);

    private const float interval = 0.04f;
    private const float delta = 0.45f;
    private const float duration = 0.2f;

    private float currentDuration = 0f;
    private float timeInterval = 0f;
    private float time = 0f;

    private Vector3 camPos = Vector3.zero;
    private bool settled = true;
    
    public void Shake(Vector3 caller, float strength = 1f) {
        Debug.Log("shake");
        if (strengths.Count >= 5) {
            return;
        }
        float dist = Vector3.Distance(envCamera.position, caller);
        strengths.Add(Mathf.Lerp(43, 1, dist)*strength);
        var diff = strengths[strengths.Count-1]*duration-currentDuration;
        if (diff < 0) 
            diff = 0;
        currentDuration += diff;
        settled = false;
        if (strengths.Count == 1) {
            var delta = DeltaCollective();
            camPos = new Vector3(Random.Range(delta/2f, delta),Random.Range(delta/2f, delta));
        }
        enabled = true;
    }

    private float DeltaCollective() {
        var delta = 0f;
        foreach (var str in strengths.ToArray()) {
            delta += str*CameraShake.delta;
        }
        return delta;
    }

    private float lerpSpeed;

    private void Update() {
        timeInterval += Time.deltaTime;
        time += Time.deltaTime;
        
        if (time >= currentDuration) {
            camPos = Vector3.zero;
            strengths.Clear();
        }else if (timeInterval >= interval) {
            timeInterval = 0f;
            var delta = DeltaCollective();
            var angle = Mathf.Atan2(-camPos.normalized.y*delta, -camPos.normalized.x*delta);
            angle += Mathf.Deg2Rad*Random.Range(0, 90f);
            camPos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
        }

        for (int i = strengths.Count-1; i >= 0; i--) {
            if (time >= strengths[i]*duration) {
                strengths.RemoveAt(i);
            }
        }

        lerpSpeed = Mathf.Lerp(currentDuration, 0f, time)*30f;
   
        if (strengths.Count == 0 && !settled) {
            envCamera.localPosition = Vector3.Lerp(envCamera.localPosition, Vector3.zero, Time.deltaTime*50f);
            if (envCamera.localPosition == Vector3.zero) {
                settled = true;
                enabled = false;
                time = 0f;
                timeInterval = 0f;
                currentDuration = 0f;
                return;
            }
        }

        envCamera.localPosition = Vector3.Lerp(envCamera.localPosition, camPos, Time.deltaTime*lerpSpeed);
    }
}
