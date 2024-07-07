using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookTo : MonoBehaviour
{
    [SerializeField] private GameObject looker;
    [SerializeField] private Option<GameObject> target;
    [SerializeField,Range(4f,15)] private float lerpSpeed;

    private Quaternion lookRotation;

    private void Awake() {
        if (target == null) {
            Debug.LogError("Assign the target on "+gameObject.name+"dumbass",gameObject);
            enabled = false;
        } 
        if (looker == null) {
            looker = gameObject;
        }
    }

    private void Update() {
        if (target.IsNone()) {
            return;
        }
        lookRotation = Quaternion.LookRotation(target.Get().transform.position-looker.transform.position);
        looker.transform.rotation = Quaternion.Lerp(looker.transform.rotation, lookRotation, Time.deltaTime*lerpSpeed);
    }
}
