using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookTo : MonoBehaviour
{
    [SerializeField] private Stats stats;
    [SerializeField] private GameObject looker;
    [SerializeField,Range(4f,15)] private float lerpSpeed;

    private Quaternion lookRotation;

    private void Awake() {

        if (looker == null) {
            looker = gameObject;
        }
    }

    private void Update() {
        if (!stats.TargetIsValid()) {
            stats.SetTarget();
            return;
        }
        lookRotation = Quaternion.LookRotation(stats.Target.transform.position-looker.transform.position);
        looker.transform.rotation = Quaternion.Lerp(looker.transform.rotation, lookRotation, Time.deltaTime*lerpSpeed);
    }

    public bool isTargeting() {
        return stats.TargetIsValid();
    }
}
